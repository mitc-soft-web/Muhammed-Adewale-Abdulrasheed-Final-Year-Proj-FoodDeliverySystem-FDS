using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int Restaurants { get; set; }

        public int Categories { get; set; }

        public int FoodItems { get; set; }

        public int Orders { get; set; }

        public int NewOrders { get; set; }

        public int SuccessfulPayments { get; set; }

        public int FailedPayments { get; set; }

        public int AvailableRiders { get; set; }

        public int TodayOrders { get; set; }

        public decimal TodayRevenue { get; set; }

        public int OpenRestaurants { get; set; }

        public List<FoodDeliverySystem.Models.Order> RecentOrders { get; set; } = new();

        public List<AdminActivityDto> RecentActivities { get; set; } = new();

        public List<TopFoodDto> TopFoods { get; set; } = new();

        public List<string> SalesLabels { get; set; } = new();

        public List<decimal> SalesRevenue { get; set; } = new();

        public List<int> OrderChart { get; set; } = new();

        public List<int> PaymentChart { get; set; } = new();

        public int PendingOrders { get; set; }

        public int PreparingOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int CancelledOrders { get; set; }
    }

    public class TopFoodDto
    {
        public string Food { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}