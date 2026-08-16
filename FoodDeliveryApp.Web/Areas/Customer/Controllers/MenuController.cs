using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.ViewModels.Category;
using FoodDeliveryApp.Application.ViewModels.Menu;
using FoodDeliveryApp.Application.ViewModels.MenuItem;
using FoodDeliveryApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDeliveryApp.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MenuController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShoppingCartRepository _cartRepository;

        public MenuController(IMenuItemRepository menuItemRepository ,
                              ICategoryRepository categoryRepository ,
                              IShoppingCartRepository cartRepository)
        {
            _menuItemRepository = menuItemRepository;
            _categoryRepository = categoryRepository;
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null , string? categoryName = null)
        {
            ViewBag.CategoryName = categoryName;
            ViewBag.Search = search;

            var categories = await _categoryRepository.GetAllAsync();
            var menuItems = await _menuItemRepository.GetAllAsync(search , categoryName);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCart? cart = null;
            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _cartRepository.GetCartAsync(userId);
            }

            var categoryVMs = categories.Select(category => new CategoryResponseVM
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            }).ToList();

            var menuItemVMs = menuItems.Select(menuItem => new MenuItemResponseVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                ImageUrl = menuItem.ImageUrl,
                Price = menuItem.Price,
                SpecialPrice = menuItem.SpecialPrice,
                CategoryName = menuItem.Category.Name,
                QuantityInCart = cart?.Items.FirstOrDefault(m => m.MenuItemId == menuItem.Id)?.Quantity ?? 0
            }).ToList();

            var menuVm = new MenuListVM
            {
                Categories = categoryVMs,
                MenuItems = menuItemVMs
            };

            return View(menuVm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if(menuItem == null)
            {
                return NotFound();
            }

            var vm = new MenuItemResponseVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                ImageUrl = menuItem.ImageUrl,
                Price = menuItem.Price,
                SpecialPrice = menuItem.SpecialPrice,
                CategoryName = menuItem.Category.Name
            };

            return View(vm);
        }
    }
}
