using OrderProcessing.Domain.Models;
using System;

namespace OrderProcessing.Domain.Operations
{
    public class PlaceOrderOperation
    {
        public PlacedOrder Execute(PricedOrder pricedOrder)
        {
            return new PlacedOrder(
                pricedOrder.OrderId,
                pricedOrder.CustomerInfo,
                pricedOrder.OrderLines,
                pricedOrder.TotalAmount,
                DateTime.UtcNow
            );
        }
    }
}
