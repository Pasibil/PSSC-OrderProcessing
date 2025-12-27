using System;
using System.Text.Json;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using OrderProcessing.Events.Models;

namespace OrderProcessing.Events
{
    public abstract class AbstractEventHandler<T> : IEventHandler
    {
        public abstract string[] EventTypes { get; }

        public async Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent)
        {
            T eventData;
            
            if (cloudEvent.Data is JsonElement jsonElement)
            {
                eventData = JsonSerializer.Deserialize<T>(jsonElement.GetRawText())!;
            }
            else if (cloudEvent.Data is T typedData)
            {
                eventData = typedData;
            }
            else
            {
                throw new InvalidOperationException($"Expected event data of type {typeof(T).Name}, got {cloudEvent.Data?.GetType().Name}");
            }

            return await OnHandleAsync(eventData);
        }

        protected abstract Task<EventProcessingResult> OnHandleAsync(T eventData);
    }
}
