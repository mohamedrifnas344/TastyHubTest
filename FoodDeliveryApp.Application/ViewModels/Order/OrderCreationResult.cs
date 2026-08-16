using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.ViewModels.Order
{
    public class OrderCreationResult
    {
        public Guid OrderId { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
