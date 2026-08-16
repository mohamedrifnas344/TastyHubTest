using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Application.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryApp.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestVM requestVM)
        {
            if (!ModelState.IsValid)
            {
                return View(requestVM);
            }

            var result = await _authService.Register(requestVM);
            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Registration failed.";
                return View(requestVM);
            }

            TempData["success"] = result.Message ?? "Registration success";
            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestVM requestVM)
        {
            if (!ModelState.IsValid)
            {
                return View(requestVM);
            }

            var result = await _authService.Login(requestVM);
            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Login failed.";
                return View(requestVM);
            }

            TempData["success"] = result.Message ?? "Login successful";

            if(result.Roles?.Contains("ADMIN") == true)
            {
                return RedirectToAction(nameof(Index), "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            TempData["success"] = "Logged out successfully";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
