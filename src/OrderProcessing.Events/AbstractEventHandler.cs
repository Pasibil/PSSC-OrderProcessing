using System;
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
            if (cloudEvent.Data is not T eventData)
            {
                throw new InvalidOperationException($"Expected event data of type {typeof(T).Name}");
            }

            return await OnHandleAsync(eventData);
        }

        protected abstract Task<EventProcessingResult> OnHandleAsync(T eventData);
    }
}
