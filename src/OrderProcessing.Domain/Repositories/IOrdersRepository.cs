using OrderProcessing.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderProcessing.Domain.Repositories
{
    public interface IOrdersRepository
    {
        Task<OrderId> SaveOrderAsync(PlacedOrder order);
        Task<List<PlacedOrder>> GetAllOrdersAsync();
        Task<PlacedOrder?> GetOrderByIdAsync(OrderId orderId);
    }
}
