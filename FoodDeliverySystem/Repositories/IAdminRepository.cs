using FoodDeliverySystem.DTOs.Admin;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IAdminRepository
    {
        Task ExpireUnpaidPaymentsAsync();

        Task<int> GetRestaurantCountAsync();

        Task<int> GetCategoryCountAsync();

        Task<int> GetFoodItemCountAsync();

        Task<int> GetOrderCountAsync();

        Task<int> GetPendingOrderCountAsync();

        Task<int> GetSuccessfulPaymentCountAsync();

        Task<int> GetFailedPaymentCountAsync();

        Task<int> GetAvailableRiderCountAsync();

        Task<int> GetTodayOrderCountAsync();

        Task<decimal> GetTodayRevenueAsync();

        Task<int> GetOpenRestaurantCountAsync();

        Task<List<Order>> GetRecentOrdersAsync(int count);

        Task<List<Order>> GetRecentOrdersForActivityAsync(int count);

        Task<List<Payment>> GetRecentSuccessfulPaymentsAsync(int count);

        Task<List<Restaurant>> GetRecentRestaurantsAsync(int count);

        Task<List<DeliveryRider>> GetRecentRidersAsync(int count);

        Task<List<TopFoodDto>> GetTopFoodsAsync(int count);

        Task<decimal> GetRevenueForDayAsync(DateTime day);

        Task<int> GetOrdersForDayAsync(DateTime day);

        Task<int> GetSuccessfulPaymentsForDayAsync(DateTime day);

        Task<int> GetPreparingOrderCountAsync();

        Task<int> GetDeliveredOrderCountAsync();

        Task<int> GetCancelledOrderCountAsync();

        Task<AdminSettings?> GetSettingsAsync(string userId);

        Task<AdminSettings> GetOrCreateSettingsAsync(string userId);

        Task SaveSettingsAsync(AdminSettings settings);

        Task<ApplicationUser?> GetUserAsync(string userId);

        Task<ApplicationUser?> FindUserByEmailAsync(string email);

        Task UpdateUserAsync(ApplicationUser user);

        Task<List<Notification>> GetNotificationsAsync(
            string filter,
            string search,
            int page,
            int pageSize);

        Task<int> GetNotificationCountAsync(
            string filter,
            string search);

        Task<int> GetUnreadNotificationCountAsync();

        Task MarkAllNotificationsReadAsync();
    }
}