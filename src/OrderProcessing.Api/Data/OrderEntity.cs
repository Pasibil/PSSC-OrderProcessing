using System;
using System.Collections.Generic;

namespace OrderProcessing.Api.Data
{
    public class OrderEntity
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime PlacedAt { get; set; }
        
        public List<OrderLineEntity> OrderLines { get; set; } = new();
    }

    public class OrderLineEntity
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
        
        public OrderEntity Order { get; set; } = null!;
    }
}
