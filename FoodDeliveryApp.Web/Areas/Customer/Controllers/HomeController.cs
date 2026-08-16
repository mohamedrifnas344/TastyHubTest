using FoodDeliveryApp.Application.Error;
using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.ViewModels.Category;
using FoodDeliveryApp.Application.ViewModels.Home;
using FoodDeliveryApp.Application.ViewModels.MenuItem;
using FoodDeliveryApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FoodDeliveryApp.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IShoppingCartRepository _cartRepository;

        public HomeController(ICategoryRepository categoryRepository , 
                              IMenuItemRepository menuItemRepository ,
                              IShoppingCartRepository cartRepository)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var menuItems = await _menuItemRepository.GetAllAsync();

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

            var vm = new HomeListVM
            {
                Categories = categoryVMs,
                MenuItems = menuItemVMs,
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(string? search = null , string? categoryName = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCart? cart = null;
            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _cartRepository.GetCartAsync(userId);
            }

            var menuItems = await _menuItemRepository.GetAllAsync(search, categoryName);
            var menuItemsVMs = menuItems.Select(menuItem => new MenuItemResponseVM
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

            return PartialView("_MenuItemCard", menuItemsVMs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
