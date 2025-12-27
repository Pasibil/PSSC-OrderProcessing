using OrderProcessing.Events;

namespace OrderProcessing.Shipping.Worker
{
    public class EventProcessorWorker : BackgroundService
    {
        private readonly IEventListener eventListener;
        private readonly ILogger<EventProcessorWorker> logger;

        public EventProcessorWorker(
            IEventListener eventListener,
            ILogger<EventProcessorWorker> logger)
        {
            this.eventListener = eventListener;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Shipping Worker starting...");
            await eventListener.StartAsync("orders", "shipping-subscription", stoppingToken);
            logger.LogInformation("Shipping Worker started and listening for events");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Shipping Worker stopping...");
            await eventListener.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
