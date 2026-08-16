using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Application.ViewModels.Account;
using FoodDeliveryApp.Application.ViewModels.UserManagement;
using FoodDeliveryApp.Domain.Entities;
using FoodDeliveryApp.Utility.IdentityHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthService(UserManager<AppUser> userManager ,
                           SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<UserResponseVM>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.Where(u => u.Email != Roles.AdminEmail)
                .ToListAsync();
            var userList = new List<UserResponseVM>();
            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserResponseVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Emain = user.Email ?? "N/A",
                    UserName = user.Email ?? "N/A",
                    Role = roles.FirstOrDefault() ?? "N/A"
                });
            }
            return userList;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _userManager.Users.CountAsync(u => u.Email != Roles.AdminEmail);
        }

        public async Task<AuthResult> Login(LoginRequestVM requestVM)
        {
            var user = await _userManager.FindByEmailAsync(requestVM.Email);
            if(user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Emain or Password is incorrect. please check and try again."
                };
            }

            var userResult = await _signInManager.PasswordSignInAsync(user, requestVM.Password,
                                                                      isPersistent: true, lockoutOnFailure: false);
            if (userResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return new AuthResult
                {
                    Success = true,
                    Message = "Login successful",
                    Roles = roles
                };
            }

            return new AuthResult
            {
                Success = false,
                Message = "Email or Password is in correct. please check and tyr again."
            };
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthResult> Register(RegisterRequestVM requestVM)
        {
            var user = await _userManager.FindByEmailAsync(requestVM.Email);
            if(user != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Email is already in use"
                };
            }

            var identityUser = new AppUser
            {
                FirstName = requestVM.FirstName,
                LastName = requestVM.LastName,
                Email = requestVM.Email,
                UserName = requestVM.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, requestVM.Password);
            if (!identityResult.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Registration failed"
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(identityUser, Roles.User);
            if (!roleResult.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Role assignment failed"
                };
            }

            return new AuthResult
            {
                Success = true,
                Message = "Registration completed successfully"
            };
        }

        public async Task<UserResponseVM> UserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Email ?? "N/A",
                Emain = user.Email ?? "N/A",
                Role = roles.FirstOrDefault() ?? "N/A"
            };
        }
    }
}
