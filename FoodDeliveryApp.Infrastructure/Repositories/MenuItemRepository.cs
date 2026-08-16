using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MenuItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(MenuItem menuItem)
        {
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.MenuItems.CountAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var menuItem = await _dbContext.MenuItems.FindAsync(id);
            if(menuItem == null)
            {
                return;
            }
            _dbContext.MenuItems.Remove(menuItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync(string? search = null, string? categoryName = null)
        {
            var query = _dbContext.MenuItems
                .Include(q => q.Category)
                .AsQueryable();

            //Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(m => m.Name.Contains(search));
            }

            //Filtering by category
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query = query.Where(c => c.Category.Name.ToLower() == categoryName.ToLower());
            }

            return await query.ToListAsync();
        }

        public async Task<MenuItem?> GetAsync(Guid id)
        {
            return await _dbContext.MenuItems
                .Include(q => q.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
