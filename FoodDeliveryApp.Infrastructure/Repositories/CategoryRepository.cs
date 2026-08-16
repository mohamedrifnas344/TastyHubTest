using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Categories.CountAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
            {
                return;
            }
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
           return await _dbContext.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetAsync(Guid id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            var categoryFromDb = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
            if(categoryFromDb == null)
            {
                return;
            }

            categoryFromDb.Name = category.Name;
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                categoryFromDb.ImageUrl = category.ImageUrl;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
