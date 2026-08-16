using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Domain.Entities
{
    public class ShoppingCartItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; } = 1;

        public Guid MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;

        public Guid ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; } = null!;
    }
}
