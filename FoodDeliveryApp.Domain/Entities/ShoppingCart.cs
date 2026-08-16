using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Domain.Entities
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
