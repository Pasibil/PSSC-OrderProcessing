using Azure.Messaging.ServiceBus;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.Extensions.Logging;
using OrderProcessing.Events;
using OrderProcessing.Events.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing.Events.ServiceBus
{
    public class ServiceBusTopicEventListener : IEventListener
    {
        private ServiceBusProcessor? processor;
        private readonly ServiceBusClient client;
        private readonly Dictionary<string, IEventHandler> eventHandlers;
        private readonly ILogger<ServiceBusTopicEventListener> logger;
        private readonly JsonEventFormatter formatter = new();

        public ServiceBusTopicEventListener(
            ServiceBusClient client,
            ILogger<ServiceBusTopicEventListener> logger,
            IEnumerable<IEventHandler> eventHandlers)
        {
            this.client = client;
            this.eventHandlers = eventHandlers.SelectMany(handler => handler.EventTypes
                                                                            .Select(eventType => (eventType, handler)))
                                                            .ToDictionary(pair => pair.eventType, pair => pair.handler);
            this.logger = logger;
        }

        public Task StartAsync(string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            ServiceBusProcessorOptions options = new()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 2
            };

            processor = client.CreateProcessor(topicName, subscriptionName, options);
            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            return processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (processor != null)
            {
                await processor.StopProcessingAsync(cancellationToken);
                processor.ProcessMessageAsync -= Processor_ProcessMessageAsync;
                processor.ProcessErrorAsync -= Processor_ProcessErrorAsync;
            }
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            logger.LogError(arg.Exception, $"{arg.ErrorSource}, {arg.FullyQualifiedNamespace}, {arg.EntityPath}");
            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            if (await EnsureMaxDeliveryCountAsync(arg))
            {
                await ProcessMessageAsCloudEventAsync(arg);
            }
        }

        private async Task<bool> EnsureMaxDeliveryCountAsync(ProcessMessageEventArgs arg)
        {
            bool canContinue = true;
            if (arg.Message.DeliveryCount > 5)
            {
                logger.LogError($"Retry count exceeded {arg.Message.MessageId}");
                await arg.DeadLetterMessageAsync(arg.Message, "Retry count exceeded");
                canContinue = false;
            }
            return canContinue;
        }

        private async Task ProcessMessageAsCloudEventAsync(ProcessMessageEventArgs arg)
        {
            CloudEvent cloudEvent = formatter.DecodeStructuredModeMessage(arg.Message.Body, null, null);
            logger.LogInformation($"Received cloud event {cloudEvent.Id} of type {cloudEvent.Type}");

            EventProcessingResult processingResult;

            if (eventHandlers.TryGetValue(cloudEvent.Type!, out IEventHandler? eventHandler))
            {
                try
                {
                    processingResult = await eventHandler.HandleAsync(cloudEvent);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error handling event {cloudEvent.Id}");
                    processingResult = EventProcessingResult.Abandoned;
                }
            }
            else
            {
                logger.LogWarning($"No handler found for event type {cloudEvent.Type}");
                processingResult = EventProcessingResult.DeadLettered;
            }

            await HandleProcessingResultAsync(arg, processingResult);
        }

        private async Task HandleProcessingResultAsync(ProcessMessageEventArgs arg, EventProcessingResult result)
        {
            switch (result)
            {
                case EventProcessingResult.Completed:
                    await arg.CompleteMessageAsync(arg.Message);
                    break;
                case EventProcessingResult.Abandoned:
                    await arg.AbandonMessageAsync(arg.Message);
                    break;
                case EventProcessingResult.DeadLettered:
                    await arg.DeadLetterMessageAsync(arg.Message);
                    break;
            }
        }
    }
}
