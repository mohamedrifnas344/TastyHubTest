using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.MenuItem
{
    public class MenuItemAddVM
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100 , MinimumLength = 2 , ErrorMessage = "Length must be between 2 - 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Length must be between 10 - 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [DisplayName("Image")]
        public IFormFile File { get; set; }

        [Required]
        [Range(0 , 100)]
        public decimal Price { get; set; }

        [Range(0 , 100)]
        [DisplayName("Special Price")]
        public decimal? SpecialPrice { get; set; }

        [Required]
        [DisplayName("Category")]
        public Guid CategoryId { get; set; }
    }
}
