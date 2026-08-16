using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Domain.Enums;
using FoodDeliveryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null)
        {
            var query = _dbContext.Orders
                .Include(q => q.Items)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(o => o.OrderStatus == status.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _dbContext.Orders
                .Include(q => q.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _dbContext.Orders
                .Include(q => q.Items)
                .Where(x => x.AppUserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                return;
            }
            order.OrderStatus = status;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatusAsync(Guid orderId, PaymentStatus status)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                return;
            }
            order.PaymentStatus = status;
            if(status == PaymentStatus.Success)
            {
                order.PaymentDate = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStripeSessionIdAsync(Guid orderId, string sessionId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if(order == null)
            {
                return;
            }
            order.SessionId = sessionId;
            await _dbContext.SaveChangesAsync();
        }
    }
}
