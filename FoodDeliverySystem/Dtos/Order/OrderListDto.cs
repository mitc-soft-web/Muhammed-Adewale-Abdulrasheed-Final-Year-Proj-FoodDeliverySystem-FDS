using System.Collections.Generic;

namespace FoodDeliverySystem.DTOs.Order
{
    public class OrderListDto
    {
        public List<OrderDto> Orders { get; set; }
            = new();

        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }

        public int PreparingOrders { get; set; }

        public int DeliveryOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int TotalPages { get; set; }
    }
}