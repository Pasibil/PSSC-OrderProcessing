using System;
using System.Collections.Generic;

namespace OrderProcessing.Invoicing.Models
{
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderLineDto> OrderLines { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime PlacedAt { get; set; }
    }

    public class OrderLineDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class Invoice
    {
        public Guid InvoiceId { get; set; }
        public Guid OrderId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<InvoiceLineItem> LineItems { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public DateTime GeneratedAt { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    public class InvoiceLineItem
    {
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public enum InvoiceStatus
    {
        Draft,
        Generated,
        Sent,
        Paid
    }
}
