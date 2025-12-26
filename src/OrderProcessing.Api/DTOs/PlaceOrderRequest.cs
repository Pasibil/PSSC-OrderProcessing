using System.Collections.Generic;

namespace OrderProcessing.Api.DTOs
{
    public class PlaceOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderLineDto> OrderLines { get; set; } = new();
    }
}
