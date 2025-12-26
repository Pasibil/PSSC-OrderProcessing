using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderProcessing.Api.Repositories
{
    public class InMemoryProductsRepository : IProductsRepository
    {
        private readonly Dictionary<string, decimal> _products = new()
        {
            { "LAPTOP-001", 3499.99m },
            { "MOUSE-USB-05", 49.99m },
            { "KEYBOARD-MECH", 299.99m },
            { "MONITOR-27", 1299.99m },
            { "HEADSET-WIRELESS", 199.99m },
            { "WEBCAM-HD", 149.99m },
            { "DESK-LAMP", 79.99m },
            { "CHAIR-ERGONOMIC", 899.99m },
            { "SSD-1TB", 399.99m },
            { "RAM-16GB", 249.99m }
        };

        public Task<bool> ProductExistsAsync(ProductCode code)
        {
            return Task.FromResult(_products.ContainsKey(code.Value));
        }

        public Task<Price> GetProductPriceAsync(ProductCode code)
        {
            if (_products.TryGetValue(code.Value, out var price))
            {
                return Task.FromResult(new Price(price));
            }

            throw new KeyNotFoundException($"Product with code '{code.Value}' not found");
        }
    }
}
