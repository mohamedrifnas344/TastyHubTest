using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Category
{
    public class CategoryResponseVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
