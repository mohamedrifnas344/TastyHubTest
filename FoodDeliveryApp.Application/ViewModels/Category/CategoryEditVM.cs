using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Category
{
    public class CategoryEditVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20 , MinimumLength = 3 , ErrorMessage = "Length must be between 3 - 20 characters")]
        public string Name { get; set; }

        [DisplayName("Image")]
        public IFormFile? File { get; set; }
        public string? ExistingImageUrl { get; set; }
    }
}
