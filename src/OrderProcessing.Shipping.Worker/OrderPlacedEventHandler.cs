using CloudNative.CloudEvents;
using OrderProcessing.Dto;
using OrderProcessing.Events;
using OrderProcessing.Events.Models;
using System.Net.Http.Json;

namespace OrderProcessing.Shipping.Worker
{
    public class OrderPlacedEventHandler : AbstractEventHandler<OrderPlacedEvent>
    {
        private readonly ILogger<OrderPlacedEventHandler> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public OrderPlacedEventHandler(
            ILogger<OrderPlacedEventHandler> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public override string[] EventTypes => new[] { nameof(OrderPlacedEvent) };

        protected override async Task<EventProcessingResult> OnHandleAsync(OrderPlacedEvent eventData)
        {
            try
            {
                logger.LogInformation("Processing OrderPlacedEvent for Order {OrderId}", eventData.OrderId);

                // Call Shipping API to create shipment
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("http://localhost:5261");

                var shippingRequest = new
                {
                    OrderId = eventData.OrderId,
                    CustomerName = eventData.CustomerName,
                    ShipmentLines = eventData.OrderLines.Select(line => new
                    {
                        line.ProductCode,
                        line.Quantity
                    }).ToList()
                };

                var response = await httpClient.PostAsJsonAsync("/api/shipping", shippingRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Successfully created shipment for Order {OrderId}", eventData.OrderId);
                    return EventProcessingResult.Completed;
                }
                else
                {
                    logger.LogError("Failed to create shipment for Order {OrderId}. Status: {StatusCode}", 
                        eventData.OrderId, response.StatusCode);
                    return EventProcessingResult.Abandoned;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing OrderPlacedEvent for Order {OrderId}", eventData.OrderId);
                return EventProcessingResult.Abandoned;
            }
        }
    }
}
