using CloudNative.CloudEvents;
using OrderProcessing.Dto;
using OrderProcessing.Events;
using OrderProcessing.Events.Models;
using System.Net.Http.Json;

namespace OrderProcessing.Invoicing.Worker
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

                // Call Invoicing API to generate invoice
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("http://localhost:5260");

                var invoiceRequest = new
                {
                    OrderId = eventData.OrderId,
                    CustomerName = eventData.CustomerName,
                    CustomerEmail = eventData.CustomerEmail,
                    InvoiceLines = eventData.OrderLines.Select(line => new
                    {
                        line.ProductCode,
                        line.Quantity,
                        line.UnitPrice
                    }).ToList(),
                    TotalAmount = eventData.TotalAmount
                };

                var response = await httpClient.PostAsJsonAsync("/api/invoices", invoiceRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Successfully generated invoice for Order {OrderId}", eventData.OrderId);
                    return EventProcessingResult.Completed;
                }
                else
                {
                    logger.LogError("Failed to generate invoice for Order {OrderId}. Status: {StatusCode}", 
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
