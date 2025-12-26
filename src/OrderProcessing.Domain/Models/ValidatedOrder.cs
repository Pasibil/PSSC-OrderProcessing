using System.Collections.Generic;

namespace OrderProcessing.Domain.Models
{
    public record ValidatedOrder(
        OrderId OrderId,
        CustomerInfo CustomerInfo,
        List<ValidatedOrderLine> OrderLines
    );
}
