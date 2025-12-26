using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.DTOs;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Workflows;
using System.Linq;
using System.Threading.Tasks;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly PlaceOrderWorkflow _placeOrderWorkflow;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            PlaceOrderWorkflow placeOrderWorkflow,
            ILogger<OrdersController> logger)
        {
            _placeOrderWorkflow = placeOrderWorkflow;
            _logger = logger;
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="request">Order details</param>
        /// <returns>Order placement result</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PlaceOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlaceOrderResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                // Map DTO to Domain Command
                var command = new PlaceOrderCommand(
                    request.CustomerName,
                    request.CustomerEmail,
                    request.OrderLines.Select(line => new UnvalidatedOrderLine(
                        line.ProductCode,
                        line.Quantity
                    )).ToList()
                );

                // Execute workflow
                var result = await _placeOrderWorkflow.ExecuteAsync(command);

                // Map result to DTO
                return result switch
                {
                    OrderPlacedSuccessEvent success => Ok(new PlaceOrderResponse
                    {
                        Success = true,
                        OrderId = success.Order.OrderId.Value.ToString(),
                        OrderDetails = new OrderDetailsDto
                        {
                            OrderId = success.Order.OrderId.Value.ToString(),
                            CustomerName = success.Order.CustomerInfo.Name,
                            CustomerEmail = success.Order.CustomerInfo.Email,
                            OrderLines = success.Order.OrderLines.Select(line => new PricedOrderLineDto
                            {
                                ProductCode = line.ProductCode.Value,
                                Quantity = line.Quantity.Value,
                                Price = line.Price.Value,
                                LineTotal = line.LineTotal.Value
                            }).ToList(),
                            TotalAmount = success.Order.TotalAmount.Value,
                            PlacedAt = success.Order.PlacedAt
                        }
                    }),
                    OrderPlacedFailedEvent failure => BadRequest(new PlaceOrderResponse
                    {
                        Success = false,
                        ErrorMessage = failure.Reason
                    }),
                    _ => StatusCode(500, new PlaceOrderResponse
                    {
                        Success = false,
                        ErrorMessage = "Unknown error occurred"
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                return StatusCode(500, new PlaceOrderResponse
                {
                    Success = false,
                    ErrorMessage = $"Internal server error: {ex.Message}"
                });
            }
        }
    }
}
