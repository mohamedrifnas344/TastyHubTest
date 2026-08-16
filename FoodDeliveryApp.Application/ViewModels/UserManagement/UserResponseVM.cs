using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.UserManagement
{
    public class UserResponseVM
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Emain { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
