namespace OrderProcessing.Domain.Models
{
    public record ValidatedOrderLine(
        ProductCode ProductCode,
        Quantity Quantity
    );
}
