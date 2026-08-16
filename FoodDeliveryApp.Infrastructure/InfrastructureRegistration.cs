using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Infrastructure.Data;
using FoodDeliveryApp.Infrastructure.Repositories;
using FoodDeliveryApp.Infrastructure.Services.Auth;
using FoodDeliveryApp.Infrastructure.Services.CloudinaryService;
using FoodDeliveryApp.Infrastructure.Services.StripePayment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMenuItemRepository , MenuItemRepository>();
            services.AddScoped<IShoppingCartRepository , ShoppingCartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICloudinaryImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<IAuthService, AuthService>();

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            return services;
        }
    }
}
