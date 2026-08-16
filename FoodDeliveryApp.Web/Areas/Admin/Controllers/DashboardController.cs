using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Application.ViewModels.Dashboard;
using FoodDeliveryApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class DashboardController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAuthService _authService;

        public DashboardController(ICategoryRepository categoryRepository ,
                                   IMenuItemRepository menuItemRepository ,
                                   IOrderRepository orderRepository ,
                                   IAuthService authService)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
            _orderRepository = orderRepository;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();

            var today = DateTime.Now;
            var monthlyRevenue = new List<decimal>();
            var monthlyOrders = new List<int>();

            for(int i = 5; i >= 0; i--)
            {
                var target = today.AddMonths(-i);
                var monthlyData = orders.Where(o => o.OrderDate.Month == target.Month &&
                                               o.OrderDate.Year == target.Year);
                monthlyRevenue.Add(monthlyData.Sum(o => o.GrandTotal));
                monthlyOrders.Add(monthlyData.Count());
            }

            var totalMenuItems = await _menuItemRepository.CountAsync();
            var totalCategories = await _categoryRepository.CountAsync();
            var totalOrders = orders.Count();

            var pendingOrders = orders.Count(o => o.OrderStatus == OrderStatus.Pending);
            var confirmOrders = orders.Count(o => o.OrderStatus == OrderStatus.Confirmed);
            var preparingOrders = orders.Count(o => o.OrderStatus == OrderStatus.Preparing);
            var outForDeliveryOrders = orders.Count(o => o.OrderStatus == OrderStatus.OutForDelivery);
            var deliveredOrders = orders.Count(o => o.OrderStatus == OrderStatus.Delivered);
            var cancellOrders = orders.Count(o => o.OrderStatus == OrderStatus.Cancelled);

            var pendingPayments = orders.Count(o => o.PaymentStatus == PaymentStatus.Pending);
            var confirmPayments = orders.Count(o => o.PaymentStatus == PaymentStatus.Success);
            var failPayments = orders.Count(o => o.PaymentStatus == PaymentStatus.Failed);

            var totalRevenu = orders.Sum(o => o.GrandTotal);
            var totalUsers = await _authService.GetUserCountAsync();

            var dashboardVM = new DahsboardVM
            {
                TotalMenuItems = totalMenuItems,
                TotalCategories = totalCategories,
                TotalOrders = totalOrders,

                PendingOrders = pendingOrders,
                PreparingOrders = preparingOrders,
                ConfirmOrders = confirmOrders,
                OutForDeliveryOrders = outForDeliveryOrders,
                DeliveredOrders = deliveredOrders,
                CancellOrders = cancellOrders,

                PendingPaymnts = pendingPayments,
                ConfirmPayments = confirmPayments,
                CancellPayments = failPayments,

                TotalRevenue = totalRevenu,
                TotalUsers = totalUsers,

                MonthlyRevenue = monthlyRevenue,
                MonthlyOrders = monthlyOrders
            };

            return View(dashboardVM);
        }
    }
}
