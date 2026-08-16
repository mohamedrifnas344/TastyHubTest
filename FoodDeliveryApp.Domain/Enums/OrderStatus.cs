using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}
