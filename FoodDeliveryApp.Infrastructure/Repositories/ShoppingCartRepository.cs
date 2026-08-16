using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddShoppingCartItemAsync(string userId, Guid menuItemId, int quantity)
        {
            var cart = await GetCartWithItemAsync(userId);
            if(cart == null)
            {
                cart = new ShoppingCart
                {
                    AppUserId = userId,
                };
                await _dbContext.ShoppingCarts.AddAsync(cart);
            }
            var existingItem = cart.Items.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if(existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new ShoppingCartItem
                {
                    MenuItemId = menuItemId,
                    Quantity = quantity,
                    ShoppingCart = cart
                };
                cart.Items.Add(newItem);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _dbContext.ShoppingCartItems
                .Where(x => x.ShoppingCart.AppUserId == userId)
                .ToListAsync();
            _dbContext.ShoppingCartItems.RemoveRange(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DecrementCartItemQuantityAsync(string userId, Guid menuItemId)
        {
            var cart = await GetCartWithItemAsync(userId);
            if (cart == null)
            {
                return false;
            }
            var item = cart.Items.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if (item == null)
            {
                return false;
            }
            if(item.Quantity <= 1)
            {
                _dbContext.ShoppingCartItems.Remove(item);
            }
            item.Quantity -= 1;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ShoppingCart?> GetCartAsync(string userId)
        {
            return await _dbContext.ShoppingCarts
                .Include(q => q.Items)
                .ThenInclude(m => m.MenuItem)
                .FirstOrDefaultAsync(x => x.AppUserId == userId);
        }

        public async Task<bool> IncrementCartItemQuantityAsync(string userId, Guid menuItemId)
        {
            var cart = await GetCartWithItemAsync(userId);
            if (cart == null)
            {
                return false;
            }
            var item = cart.Items.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if(item == null)
            {
                return false;
            }
            item.Quantity += 1;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCartItemAsync(string userId, Guid menuItemId)
        {
            var cart = await GetCartWithItemAsync(userId);
            if (cart == null)
            {
                return false;
            }
            var item = cart.Items.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if (item == null)
            {
                return false;
            }

            _dbContext.ShoppingCartItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string userId, Guid menuItemId, int quantity)
        {
            var cart = await GetCartWithItemAsync(userId);
            if (cart == null)
            {
                return false;
            }
            var item = cart.Items.FirstOrDefault(m => m.MenuItemId == menuItemId);
            if(item == null)
            {
                return false;
            }

            item.Quantity = quantity < 1 ? 1 : quantity;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<ShoppingCart?> GetCartWithItemAsync(string userId)
        {
            return await _dbContext.ShoppingCarts
                .Include(q => q.Items).FirstOrDefaultAsync(x => x.AppUserId == userId);
        }
    }
}
