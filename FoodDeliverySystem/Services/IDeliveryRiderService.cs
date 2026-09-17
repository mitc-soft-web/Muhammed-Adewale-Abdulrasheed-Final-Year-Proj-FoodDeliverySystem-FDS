using FoodDeliverySystem.Models;
using FoodDeliverySystem.ViewModels;

namespace FoodDeliverySystem.Services
{
    public interface IDeliveryRiderService
    {
        // =========================================================
        // ADMIN RIDER MANAGEMENT
        // =========================================================

        Task<List<DeliveryRider>> GetAllAsync(
            string? search,
            string? status,
            string? vehicle);

        Task<DeliveryRider?> GetByIdAsync(int id);

        Task<DeliveryRider?> GetByUserIdAsync(
            string userId);

        Task AddAsync(
            DeliveryRider rider);

        Task UpdateAsync(
            DeliveryRider rider);

        Task DeleteAsync(
            int id);

        Task<bool> ApproveAsync(
            int id);

        Task<bool> RejectAsync(
            int id);


        // =========================================================
        // RIDER LOCATION
        // =========================================================

        Task<RiderLocation?> GetLocationAsync(
            int riderId);

        Task UpdateLocationAsync(
            int riderId,
            double latitude,
            double longitude);


        // =========================================================
        // RIDER AVAILABILITY
        // =========================================================

        Task ToggleAvailabilityAsync(
            string userId);


        // =========================================================
        // RIDER ORDERS
        // =========================================================

        Task<Order?> GetRiderOrderDetailsAsync(
            string userId,
            int orderId);

        Task<Order?> GetRiderReceiptAsync(
            string userId,
            int orderId);

        Task<List<Order>> GetAssignedOrdersAsync(
            string userId);

        Task<List<Order>> GetDeliveryHistoryAsync(
            string userId,
            string? search,
            string? paymentMethod,
            string? quickFilter,
            DateTime? fromDate,
            DateTime? toDate);

        Task<RiderDeliveryHistoryResult> GetDeliveryHistorySummaryAsync(
        string userId,
        string? search,
        string? paymentMethod,
        string? quickFilter,
        DateTime? fromDate,
        DateTime? toDate);

        Task<bool> AcceptOrderAsync(
            string userId,
            int orderId);

        Task<bool> CompleteOrderAsync(
            string userId,
            int orderId);


        // =========================================================
        // RIDER REGISTRATION
        // =========================================================

        Task<bool> RegisterRiderAsync(
          RiderRegisterViewModel model,
          string userId,
          string? licenseFileName);


        // =========================================================
        // DASHBOARD
        // =========================================================

        Task<RiderDashboardResult?> GetDashboardAsync(
            string userId);
    }


    public class RiderDashboardResult
    {
        public DeliveryRider Rider { get; set; } = null!;

        public int AssignedOrdersCount { get; set; }

        public int NewOrdersCount { get; set; }

        public int CompletedDeliveriesCount { get; set; }

        public List<Order> ActiveOrders { get; set; } = new();

        public List<Order> DeliveredOrders { get; set; } = new();

        // Dashboard statistics
        public int TotalDeliveries { get; set; }

        public int CashOrders { get; set; }

        public int OnlineOrders { get; set; }

        public decimal CashCollected { get; set; }

        public decimal OnlineCollected { get; set; }

        public decimal TotalEarnings { get; set; }

        public int TodayDeliveries { get; set; }

        public decimal TodayEarnings { get; set; }

        public List<Order> RecentDeliveries { get; set; } = new();
    }

    public class RiderDeliveryHistoryResult
    {
        public List<Order> Deliveries { get; set; } = new();

        public int TotalDeliveries { get; set; }

        public int CashOrders { get; set; }

        public int OnlineOrders { get; set; }

        public decimal TotalEarnings { get; set; }
    }
}