using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.ViewModels.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDeliveryApp.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "USER")]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _cartRepository;

        public ShoppingCartController(IShoppingCartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = await _cartRepository.GetCartAsync(userId);
            if(cart == null || cart.Items == null || !cart.Items.Any())
            {
                var emptyCart = new ShoppingCartResponseVM
                {
                    Id = Guid.Empty,
                    Items = new List<ShoppingCartItemResponseVM>()
                };
                return View(emptyCart);
            }

            var cartVM = new ShoppingCartResponseVM
            {
                Id = cart.Id,
                Items = cart.Items.Select(item => new ShoppingCartItemResponseVM
                {
                    Id = item.Id,
                    MenuItemId = item.MenuItem.Id,
                    MenuItemName = item.MenuItem.Name,
                    MenuItemImageUrl = item.MenuItem.ImageUrl,
                    Price = item.MenuItem.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid menuItemId , int quantity)
        {
            var userId = GetUserId();
            if(userId == null)
            {
                return Json(new { success = false, message = "You must logged in to add items to the cart" });
            }

            if(quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be at least 1" });
            }

            await _cartRepository.AddShoppingCartItemAsync(userId, menuItemId, quantity);
            return Json(new { success = true, message = "Item added to the cart successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid menuItemId , int quantity)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "You must be logged in to update cart. " });
            }
            var result = await _cartRepository.UpdateCartItemQuantityAsync(userId, menuItemId, quantity);
            if(result == false)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }
            return Json(new { success = true, message = "Cart updated successfully. " });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            var cart = await _cartRepository.GetCartAsync(userId);
            var count = cart?.Items.Sum(x => x.Quantity) ?? 0;
            return Json(new { count });
        }

        [HttpPost]
        public async Task<IActionResult> IncrementQuantity(Guid menuItemId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var result = await _cartRepository.IncrementCartItemQuantityAsync(userId, menuItemId);
            if (!result)
            {
                TempData["error"] = "Fail to update cart.";
            }
            else
            {
                TempData["success"] = "Cart updated successfully.";
            }
            return RedirectToAction("Index", "ShoppingCart", new { area = "Customer" });
        }

        [HttpPost]
        public async Task<IActionResult> DecrementQuantity(Guid menuItemId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var result = await _cartRepository.DecrementCartItemQuantityAsync(userId, menuItemId);
            if (!result)
            {
                TempData["error"] = "Fail to update cart.";
            }
            else
            {
                TempData["success"] = "Cart updated successfully.";
            }
            return RedirectToAction("Index", "ShoppingCart", new { area = "Customer" });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid menuItemId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var result = await _cartRepository.RemoveCartItemAsync(userId, menuItemId);
            if (!result)
            {
                TempData["error"] = "Fail to remove cart.";
            }
            else
            {
                TempData["success"] = "Cart removed successfully.";
            }
            return RedirectToAction("Index", "ShoppingCart", new { area = "Customer" });
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
