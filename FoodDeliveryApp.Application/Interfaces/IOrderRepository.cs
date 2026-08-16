using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task UpdateStripeSessionIdAsync(Guid orderId, string sessionId);
        Task UpdatePaymentStatusAsync(Guid orderId, PaymentStatus status);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetAllOrdersAsync(OrderStatus? status = null);
    }
}
