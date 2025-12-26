using OrderProcessing.Domain.Models;
using System;
using System.Linq;

namespace OrderProcessing.Domain.Operations
{
    public class ValidateOrderOperation
    {
        public ValidatedOrder Execute(UnvalidatedOrder unvalidatedOrder)
        {
            var orderId = OrderId.NewOrderId();
            var customerInfo = new CustomerInfo(
                unvalidatedOrder.CustomerName, 
                unvalidatedOrder.CustomerEmail
            );

            var validatedLines = unvalidatedOrder.OrderLines
                .Select(line => new ValidatedOrderLine(
                    new ProductCode(line.ProductCode),
                    new Quantity(line.Quantity)
                ))
                .ToList();

            return new ValidatedOrder(orderId, customerInfo, validatedLines);
        }
    }
}
