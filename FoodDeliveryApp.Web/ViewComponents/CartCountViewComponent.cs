using FoodDeliveryApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDeliveryApp.Web.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IShoppingCartRepository _cartRepository;

        public CartCountViewComponent(IShoppingCartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int cartCount = 0;
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                var claimUser = User as ClaimsPrincipal;
                var userId = claimUser?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var cart = await _cartRepository.GetCartAsync(userId);
                    cartCount = cart?.Items.Sum(x => x.Quantity) ?? 0;
                }
            }
            return View(cartCount);
        }
    }
}
