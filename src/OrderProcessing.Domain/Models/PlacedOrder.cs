using System;
using System.Collections.Generic;

namespace OrderProcessing.Domain.Models
{
    public record PlacedOrder(
        OrderId OrderId,
        CustomerInfo CustomerInfo,
        List<PricedOrderLine> OrderLines,
        Amount TotalAmount,
        DateTime PlacedAt
    );
}
