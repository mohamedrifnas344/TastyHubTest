using FoodDeliveryApp.Application.ViewModels.Category;
using FoodDeliveryApp.Application.ViewModels.MenuItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Menu
{
    public class MenuListVM
    {
        public ICollection<MenuItemResponseVM> MenuItems { get; set; }
        public ICollection<CategoryResponseVM> Categories { get; set; }
    }
}
