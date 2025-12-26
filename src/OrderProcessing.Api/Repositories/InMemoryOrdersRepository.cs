using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderProcessing.Api.Repositories
{
    public class InMemoryOrdersRepository : IOrdersRepository
    {
        private readonly ConcurrentDictionary<string, PlacedOrder> _orders = new();

        public Task<OrderId> SaveOrderAsync(PlacedOrder order)
        {
            var key = order.OrderId.Value.ToString();
            _orders.TryAdd(key, order);
            return Task.FromResult(order.OrderId);
        }

        public Task<List<PlacedOrder>> GetAllOrdersAsync()
        {
            return Task.FromResult(_orders.Values.ToList());
        }

        public Task<PlacedOrder?> GetOrderByIdAsync(OrderId orderId)
        {
            var key = orderId.Value.ToString();
            _orders.TryGetValue(key, out var order);
            return Task.FromResult(order);
        }
    }
}
