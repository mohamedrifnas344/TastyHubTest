using FoodDeliveryApp.Application.ViewModels.Category;
using FoodDeliveryApp.Application.ViewModels.MenuItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Home
{
    public class HomeListVM
    {
        public ICollection<CategoryResponseVM> Categories { get; set; }
        public ICollection<MenuItemResponseVM> MenuItems { get; set; }
    }
}
