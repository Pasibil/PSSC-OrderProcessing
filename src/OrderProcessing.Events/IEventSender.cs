using System.Threading.Tasks;

namespace OrderProcessing.Events
{
    public interface IEventSender
    {
        Task SendAsync<T>(string topicName, T @event);
    }
}
