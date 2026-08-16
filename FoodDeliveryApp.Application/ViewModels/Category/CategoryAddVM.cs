using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Category
{
    public class CategoryAddVM
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100 , MinimumLength = 3 , ErrorMessage = "Name must be between 2 and 100 characters")]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [DisplayName("Image")]
        public IFormFile File { get; set; }
    }
}
