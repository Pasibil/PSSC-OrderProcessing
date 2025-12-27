using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing.Events
{
    public interface IEventListener
    {
        Task StartAsync(string topicName, string subscriptionName, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
