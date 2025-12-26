using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Shipping.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OrderProcessing.Shipping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, ShippingInfo> _shipments = new();
        private static int _trackingCounter = 1000000;
        private static readonly string[] _couriers = { "Fan Courier", "DHL", "GLS", "Cargus", "Sameday" };
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(ILogger<ShippingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create shipping for order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ShippingInfo), StatusCodes.Status201Created)]
        public ActionResult<ShippingInfo> CreateShipping([FromBody] ShippingRequest request)
        {
            try
            {
                var shippingId = Guid.NewGuid();
                var trackingNumber = $"RO{Interlocked.Increment(ref _trackingCounter)}";
                var courier = _couriers[Random.Shared.Next(_couriers.Length)];

                var shipping = new ShippingInfo
                {
                    ShippingId = shippingId,
                    OrderId = request.OrderId,
                    TrackingNumber = trackingNumber,
                    CustomerName = request.CustomerName,
                    ShippingAddress = request.ShippingAddress,
                    City = request.City,
                    PostalCode = request.PostalCode,
                    Country = request.Country,
                    Status = ShippingStatus.Processing,
                    CreatedAt = DateTime.Now,
                    Courier = courier
                };

                _shipments.TryAdd(shippingId, shipping);

                _logger.LogInformation($"Shipping {trackingNumber} created for Order {request.OrderId} via {courier}");

                return CreatedAtAction(nameof(GetShipping), new { id = shippingId }, shipping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shipping");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get shipping by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ShippingInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShippingInfo> GetShipping(Guid id)
        {
            if (_shipments.TryGetValue(id, out var shipping))
            {
                return Ok(shipping);
            }
            return NotFound(new { error = $"Shipping {id} not found" });
        }

        /// <summary>
        /// Get shipping by tracking number
        /// </summary>
        [HttpGet("track/{trackingNumber}")]
        [ProducesResponseType(typeof(ShippingInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShippingInfo> GetShippingByTracking(string trackingNumber)
        {
            var shipping = _shipments.Values.FirstOrDefault(s => s.TrackingNumber == trackingNumber);
            if (shipping != null)
            {
                return Ok(shipping);
            }
            return NotFound(new { error = $"Tracking number {trackingNumber} not found" });
        }

        /// <summary>
        /// Get all shipments
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ShippingInfo>), StatusCodes.Status200OK)]
        public ActionResult<List<ShippingInfo>> GetAllShipments()
        {
            return Ok(_shipments.Values.OrderByDescending(s => s.CreatedAt).ToList());
        }

        /// <summary>
        /// Get shipments by Order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(List<ShippingInfo>), StatusCodes.Status200OK)]
        public ActionResult<List<ShippingInfo>> GetShipmentsByOrderId(Guid orderId)
        {
            var shipments = _shipments.Values
                .Where(s => s.OrderId == orderId)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return Ok(shipments);
        }

        /// <summary>
        /// Update shipping status
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ShippingInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShippingInfo> UpdateShippingStatus(Guid id, [FromBody] ShippingStatus newStatus)
        {
            if (_shipments.TryGetValue(id, out var shipping))
            {
                shipping.Status = newStatus;

                if (newStatus == ShippingStatus.Shipped && !shipping.ShippedAt.HasValue)
                {
                    shipping.ShippedAt = DateTime.Now;
                }
                else if (newStatus == ShippingStatus.Delivered && !shipping.DeliveredAt.HasValue)
                {
                    shipping.DeliveredAt = DateTime.Now;
                }

                _logger.LogInformation($"Shipping {shipping.TrackingNumber} status updated to {newStatus}");
                return Ok(shipping);
            }
            return NotFound(new { error = $"Shipping {id} not found" });
        }
    }
}
