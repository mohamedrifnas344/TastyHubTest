using FoodDeliveryApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task AddShoppingCartItemAsync(string userId, Guid menuItemId, int quantity);
        Task<ShoppingCart?> GetCartAsync(string userId);
        Task<bool> UpdateCartItemQuantityAsync(string userId, Guid menuItemId, int quantity);
        Task<bool> IncrementCartItemQuantityAsync(string userId, Guid menuItemId);
        Task<bool> DecrementCartItemQuantityAsync(string userId, Guid menuItemId);
        Task<bool> RemoveCartItemAsync(string userId, Guid menuItemId);
        Task ClearCartAsync(string userId);
    }
}
