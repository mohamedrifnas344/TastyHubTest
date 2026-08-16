using FoodDeliveryApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetAsync(Guid id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
        Task<int> CountAsync();
    }
}
