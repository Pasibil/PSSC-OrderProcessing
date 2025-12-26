using System.Collections.Generic;

namespace OrderProcessing.Domain.Models
{
    public record PlaceOrderCommand(
        string CustomerName,
        string CustomerEmail,
        List<UnvalidatedOrderLine> OrderLines
    );
}
