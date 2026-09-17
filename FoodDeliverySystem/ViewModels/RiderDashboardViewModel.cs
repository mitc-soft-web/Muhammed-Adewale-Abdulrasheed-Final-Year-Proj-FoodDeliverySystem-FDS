using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.ViewModels
{
    public class RiderDashboardViewModel
    {
        // Rider
        public DeliveryRider Rider { get; set; } = null!;

        // Statistics
        public int NewOrders { get; set; }

        public int ActiveOrders { get; set; }

        public int CompletedDeliveries { get; set; }

        // Earnings
        public decimal TotalEarnings { get; set; }

        public decimal CashCollected { get; set; }

        public decimal OnlineCollected { get; set; }

        // Order Statistics
        public int CashOrders { get; set; }

        public int OnlineOrders { get; set; }

        public int TotalDeliveries { get; set; }

        // Today's Performance
        public decimal TodayEarnings { get; set; }

        public int TodayDeliveries { get; set; }

        // Lists
        public List<Order> ActiveDeliveries { get; set; } = new();

        public List<Order> RecentDeliveries { get; set; } = new();
    }
}