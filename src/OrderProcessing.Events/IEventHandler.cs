using System.Threading.Tasks;
using CloudNative.CloudEvents;
using OrderProcessing.Events.Models;

namespace OrderProcessing.Events
{
    public interface IEventHandler
    {
        string[] EventTypes { get; }

        Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent);
    }
}
