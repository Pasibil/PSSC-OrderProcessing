using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderProcessing.Domain.Operations
{
    public class PriceOrderOperation
    {
        private readonly IProductsRepository _productsRepository;

        public PriceOrderOperation(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<PricedOrder> ExecuteAsync(ValidatedOrder validatedOrder)
        {
            var pricedLines = new List<PricedOrderLine>();

            foreach (var line in validatedOrder.OrderLines)
            {
                var price = await _productsRepository.GetProductPriceAsync(line.ProductCode);
                var lineTotal = new Amount(price.Value * line.Quantity.Value);

                pricedLines.Add(new PricedOrderLine(
                    line.ProductCode,
                    line.Quantity,
                    price,
                    lineTotal
                ));
            }

            var totalAmount = new Amount(pricedLines.Sum(l => l.LineTotal.Value));

            return new PricedOrder(
                validatedOrder.OrderId,
                validatedOrder.CustomerInfo,
                pricedLines,
                totalAmount
            );
        }
    }
}
