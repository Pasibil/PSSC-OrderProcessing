using Microsoft.Extensions.Logging;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Operations;
using OrderProcessing.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace OrderProcessing.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<PlaceOrderWorkflow> _logger;

        public PlaceOrderWorkflow(
            IProductsRepository productsRepository,
            IOrdersRepository ordersRepository,
            ILogger<PlaceOrderWorkflow> logger)
        {
            _productsRepository = productsRepository;
            _ordersRepository = ordersRepository;
            _logger = logger;
        }

        public async Task<IOrderPlacedEvent> ExecuteAsync(PlaceOrderCommand command)
        {
            try
            {
                // Step 1: Create unvalidated order
                var unvalidatedOrder = new UnvalidatedOrder(
                    command.CustomerName,
                    command.CustomerEmail,
                    command.OrderLines
                );

                // Step 2: Validate order
                var validateOperation = new ValidateOrderOperation();
                var validatedOrder = validateOperation.Execute(unvalidatedOrder);

                // Step 3: Price order
                var priceOperation = new PriceOrderOperation(_productsRepository);
                var pricedOrder = await priceOperation.ExecuteAsync(validatedOrder);

                // Step 4: Place order
                var placeOperation = new PlaceOrderOperation();
                var placedOrder = placeOperation.Execute(pricedOrder);

                // Step 5: Save to repository
                await _ordersRepository.SaveOrderAsync(placedOrder);

                _logger.LogInformation($"Order {placedOrder.OrderId} placed successfully");

                return new OrderPlacedSuccessEvent(placedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place order");
                return new OrderPlacedFailedEvent(ex.Message);
            }
        }
    }
}
