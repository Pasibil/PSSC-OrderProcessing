using System;
using System.Collections.Generic;

namespace OrderProcessing.Api.DTOs
{
    public class PlaceOrderResponse
    {
        public bool Success { get; set; }
        public string? OrderId { get; set; }
        public string? ErrorMessage { get; set; }
        public OrderDetailsDto? OrderDetails { get; set; }
    }

    public class OrderDetailsDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<PricedOrderLineDto> OrderLines { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime PlacedAt { get; set; }
    }

    public class PricedOrderLineDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
