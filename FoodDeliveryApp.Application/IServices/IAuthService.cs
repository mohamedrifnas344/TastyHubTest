using FoodDeliveryApp.Application.ViewModels.Account;
using FoodDeliveryApp.Application.ViewModels.UserManagement;
using FoodDeliveryApp.Utility.IdentityHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResult> Register(RegisterRequestVM requestVM);
        Task<AuthResult> Login(LoginRequestVM requestVM);
        Task Logout();
        Task<IEnumerable<UserResponseVM>> GetAllUsersAsync();
        Task<int> GetUserCountAsync();
        Task<UserResponseVM> UserProfileAsync(string userId);
    }
}
