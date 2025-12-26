using System;

namespace OrderProcessing.Shipping.Models
{
    public class ShippingRequest
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Romania";
    }

    public class ShippingInfo
    {
        public Guid ShippingId { get; set; }
        public Guid OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public ShippingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string Courier { get; set; } = string.Empty;
    }

    public enum ShippingStatus
    {
        Pending,
        Processing,
        Shipped,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed
    }
}
