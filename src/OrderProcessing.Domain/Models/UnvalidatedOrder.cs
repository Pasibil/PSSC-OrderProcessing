using System.Collections.Generic;

namespace OrderProcessing.Domain.Models
{
    public record UnvalidatedOrder(
        string CustomerName,
        string CustomerEmail,
        List<UnvalidatedOrderLine> OrderLines
    );
}
