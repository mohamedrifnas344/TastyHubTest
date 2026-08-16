using FoodDeliveryApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync(string? search = null, string? categoryName = null);
        Task<MenuItem?> GetAsync(Guid id);
        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync();
        Task DeleteAsync(Guid id);
        Task<int> CountAsync();
    }
}
