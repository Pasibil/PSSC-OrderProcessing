using System.Collections.Generic;

namespace OrderProcessing.Domain.Models
{
    public record PricedOrder(
        OrderId OrderId,
        CustomerInfo CustomerInfo,
        List<PricedOrderLine> OrderLines,
        Amount TotalAmount
    );
}
