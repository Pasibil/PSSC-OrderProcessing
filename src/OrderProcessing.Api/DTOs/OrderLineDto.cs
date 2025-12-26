namespace OrderProcessing.Api.DTOs
{
    public class OrderLineDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
