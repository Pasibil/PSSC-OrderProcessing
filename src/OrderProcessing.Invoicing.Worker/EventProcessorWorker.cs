using OrderProcessing.Events;

namespace OrderProcessing.Invoicing.Worker
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
            logger.LogInformation("Invoicing Worker starting...");
            await eventListener.StartAsync("orders", "invoicing-subscription", stoppingToken);
            logger.LogInformation("Invoicing Worker started and listening for events");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Invoicing Worker stopping...");
            await eventListener.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
