namespace OrderProcessing.Domain.Models
{
    public record UnvalidatedOrderLine(
        string ProductCode,
        int Quantity
    );
}
