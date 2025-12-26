using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.DTOs;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;
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
        private readonly IOrdersRepository _ordersRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            PlaceOrderWorkflow placeOrderWorkflow,
            IOrdersRepository ordersRepository,
            ILogger<OrdersController> logger)
        {
            _placeOrderWorkflow = placeOrderWorkflow;
            _ordersRepository = ordersRepository;
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

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all placed orders</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderDetailsDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _ordersRepository.GetAllOrdersAsync();
                
                var orderDtos = orders.Select(order => new OrderDetailsDto
                {
                    OrderId = order.OrderId.Value.ToString(),
                    CustomerName = order.CustomerInfo.Name,
                    CustomerEmail = order.CustomerInfo.Email,
                    OrderLines = order.OrderLines.Select(line => new PricedOrderLineDto
                    {
                        ProductCode = line.ProductCode.Value,
                        Quantity = line.Quantity.Value,
                        Price = line.Price.Value,
                        LineTotal = line.LineTotal.Value
                    }).ToList(),
                    TotalAmount = order.TotalAmount.Value,
                    PlacedAt = order.PlacedAt
                }).ToList();

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderById(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var guidId))
                {
                    return BadRequest(new { error = "Invalid order ID format" });
                }

                var orderId = OrderId.From(guidId);
                var order = await _ordersRepository.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound(new { error = $"Order with ID {id} not found" });
                }

                var orderDto = new OrderDetailsDto
                {
                    OrderId = order.OrderId.Value.ToString(),
                    CustomerName = order.CustomerInfo.Name,
                    CustomerEmail = order.CustomerInfo.Email,
                    OrderLines = order.OrderLines.Select(line => new PricedOrderLineDto
                    {
                        ProductCode = line.ProductCode.Value,
                        Quantity = line.Quantity.Value,
                        Price = line.Price.Value,
                        LineTotal = line.LineTotal.Value
                    }).ToList(),
                    TotalAmount = order.TotalAmount.Value,
                    PlacedAt = order.PlacedAt
                };

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
