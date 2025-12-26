using Microsoft.EntityFrameworkCore;
using OrderProcessing.Api.Data;
using OrderProcessing.Domain.Models;
using OrderProcessing.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderProcessing.Api.Repositories
{
    public class SqlOrdersRepository : IOrdersRepository
    {
        private readonly OrderProcessingDbContext _context;

        public SqlOrdersRepository(OrderProcessingDbContext context)
        {
            _context = context;
        }

        public async Task<OrderId> SaveOrderAsync(PlacedOrder order)
        {
            var orderEntity = new OrderEntity
            {
                Id = order.OrderId.Value,
                CustomerName = order.CustomerInfo.Name,
                CustomerEmail = order.CustomerInfo.Email,
                TotalAmount = order.TotalAmount.Value,
                PlacedAt = order.PlacedAt,
                OrderLines = order.OrderLines.Select(line => new OrderLineEntity
                {
                    OrderId = order.OrderId.Value,
                    ProductCode = line.ProductCode.Value,
                    Quantity = line.Quantity.Value,
                    Price = line.Price.Value,
                    LineTotal = line.LineTotal.Value
                }).ToList()
            };

            _context.Orders.Add(orderEntity);
            await _context.SaveChangesAsync();

            return order.OrderId;
        }

        public async Task<List<PlacedOrder>> GetAllOrdersAsync()
        {
            var entities = await _context.Orders
                .Include(o => o.OrderLines)
                .ToListAsync();

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<PlacedOrder?> GetOrderByIdAsync(OrderId orderId)
        {
            var entity = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == orderId.Value);

            return entity == null ? null : MapToDomain(entity);
        }

        private static PlacedOrder MapToDomain(OrderEntity entity)
        {
            return new PlacedOrder(
                OrderId.From(entity.Id),
                new CustomerInfo(entity.CustomerName, entity.CustomerEmail),
                entity.OrderLines.Select(line => new PricedOrderLine(
                    new ProductCode(line.ProductCode),
                    new Quantity(line.Quantity),
                    new Price(line.Price),
                    new Amount(line.LineTotal)
                )).ToList(),
                new Amount(entity.TotalAmount),
                entity.PlacedAt
            );
        }
    }
}
