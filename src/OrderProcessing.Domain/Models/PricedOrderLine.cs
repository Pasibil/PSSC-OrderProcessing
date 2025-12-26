namespace OrderProcessing.Domain.Models
{
    public record PricedOrderLine(
        ProductCode ProductCode,
        Quantity Quantity,
        Price Price,
        Amount LineTotal
    );
}
