using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.ViewModels.Order;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Domain.Enums;
using FoodDeliveryApp.Infrastructure.Services.StripePayment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace FoodDeliveryApp.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "USER")]
    public class OrderController : Controller
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly StripeSettings _stripeSettings;

        public OrderController(IShoppingCartRepository cartRepository , 
                               IOrderRepository orderRepository ,
                               IOptions<StripeSettings> options)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _stripeSettings = options.Value;
        }


        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var orderVMs = orders.Select(order => new OrderResponseVM
            {
                Id = order.Id,
                AppUserId = userId,

                TotalItems = order.TotalItems,
                DeliveryFee = order.DeliveryFee,
                TotalPrice = order.TotalPrice,
                GrandTotal = order.GrandTotal,

                OrderDate = order.OrderDate,
                PaymentDate = order.PaymentDate,

                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,

                FirstName = order.FirstName,
                LastName = order.LastName,
                Email = order.Email,
                Street = order.Street,
                City = order.City,
                PostalCode = order.PostalCode,
                Phone1 = order.Phone1,
                Phone2 = order.Phone2,
                DeliveryInstructions = order.DeliveryInstructions
            }).ToList();

            return View(orderVMs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid OrderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var order = await _orderRepository.GetOrderByIdAsync(OrderId);
            if(order == null)
            {
                return NotFound();
            }

            var vm = new OrderResponseVM
            {
                Id = order.Id,
                AppUserId = userId,

                TotalItems = order.TotalItems,
                DeliveryFee = order.DeliveryFee,
                TotalPrice = order.TotalPrice,
                GrandTotal = order.GrandTotal,

                OrderDate = order.OrderDate,
                PaymentDate = order.PaymentDate,

                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,

                Items = order.Items.Select(item => new OrderItemResponseVM
                {
                    Id = item.Id,
                    MenuItemId = item.MenuItemId,
                    MenuItemImageUrl = item.MenuItemImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    MenuItemName = item.MenuItemName
                }).ToList(),

                FirstName = order.FirstName,
                LastName = order.LastName,
                Email = order.Email,
                Street = order.Street,
                City = order.City,
                PostalCode = order.PostalCode,
                Phone1 = order.Phone1,
                Phone2 = order.Phone2,
                DeliveryInstructions = order.DeliveryInstructions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var cart = await _cartRepository.GetCartAsync(userId);
            if(cart == null || !cart.Items.Any())
            {
                return NotFound();
            }
            var vm = new OrderCreateVM
            {
                Items = cart.Items.Select(item => new OrderItemCreateVM
                {
                    MenuItemId = item.MenuItem.Id,
                    MenuItemName = item.MenuItem.Name,
                    MenuItemImageUrl = item.MenuItem.ImageUrl,
                    Price = item.MenuItem.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderCreateVM createVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (!ModelState.IsValid)
            {
                var reloadCart = await _cartRepository.GetCartAsync(userId);
                createVM.Items = reloadCart.Items.Select(item => new OrderItemCreateVM
                {
                    MenuItemId = item.MenuItem.Id,
                    MenuItemName = item.MenuItem.Name,
                    MenuItemImageUrl = item.MenuItem.ImageUrl,
                    Price = item.MenuItem.Price,
                    Quantity = item.Quantity
                }).ToList();
                return View(createVM);
            }

            var cart = await _cartRepository.GetCartAsync(userId);
            if(cart == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                AppUserId = userId,

                FirstName = createVM.FirstName,
                LastName = createVM.LastName,
                Email = createVM.Email,
                Street = createVM.Street,
                City = createVM.City,
                PostalCode = createVM.PostalCode,
                Phone1 = createVM.Phone1,
                Phone2 = createVM.Phone2,
                DeliveryInstructions = createVM.DeliveryInstructions
            };

            foreach(var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    MenuItemId = item.MenuItem.Id,
                    MenuItemName = item.MenuItem.Name,
                    MenuItemImageUrl = item.MenuItem.ImageUrl,
                    Price = item.MenuItem.Price,
                    Quantity = item.Quantity,
                    OrderId = order.Id
                };
                order.Items.Add(orderItem);
            }

            var orderTotal = cart.Items.Sum(x => x.Quantity * x.MenuItem.Price);
            var deliveryFee = 2m;
            var grandTotal = orderTotal + deliveryFee;
            var totalItems = cart.Items.Sum(x => x.Quantity);

            order.TotalPrice = orderTotal;
            order.DeliveryFee = deliveryFee;
            order.GrandTotal = grandTotal;
            order.TotalItems = totalItems;

            await _orderRepository.AddOrderAsync(order);

            //Stripe Payment
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            var domain = $"{Request.Scheme}://{Request.Host}/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Order/OrderConfirmation?orderId={order.Id}",
                CancelUrl = domain + $"Customer/Order/Cancel",
                LineItems = order.Items.Select(x => new SessionLineItemOptions
                {
                    Quantity = x.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(x.Price*100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = x.MenuItemName
                        }
                    }
                }).ToList(),
                Mode = "payment",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            await _orderRepository.UpdateStripeSessionIdAsync(order.Id, session.Id);
            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if(order == null)
            {
                return NotFound();
            }

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            var service = new SessionService();
            Session session = service.Get(order.SessionId);

            var status = session.PaymentStatus?.ToLower() switch
            {
                "paid" => PaymentStatus.Success,
                "unpaid" => PaymentStatus.Failed,
                _ => PaymentStatus.Pending
            };

            await _orderRepository.UpdatePaymentStatusAsync(orderId, status);
            if(status == PaymentStatus.Success)
            {
                await _orderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Confirmed);
                await _cartRepository.ClearCartAsync(order.AppUserId);
            }
            var orderCreationResult = new OrderCreationResult
            {
                OrderId = orderId,
                GrandTotal = order.GrandTotal
            };
            return View(orderCreationResult);
        }
    }
}
