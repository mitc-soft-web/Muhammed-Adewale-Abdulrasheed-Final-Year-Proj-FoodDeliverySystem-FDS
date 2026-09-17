using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IDeliveryRiderRepository
    {
        // Rider management
        Task<List<DeliveryRider>> GetAllAsync(
            string? search,
            string? status,
            string? vehicle);

        Task<DeliveryRider?> GetByIdAsync(int id);

        Task<DeliveryRider?> GetByUserIdAsync(string userId);

        Task AddAsync(DeliveryRider rider);

        Task UpdateAsync(DeliveryRider rider);

        Task DeleteAsync(int id);

        // Rider location
        Task<RiderLocation?> GetLocationAsync(int riderId);

        Task UpdateLocationAsync(
            int riderId,
            double latitude,
            double longitude);

        // Rider orders
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

        Task<bool> AcceptOrderAsync(
            string userId,
            int orderId);

        Task<bool> CompleteOrderAsync(
            string userId,
            int orderId);

        // Rider availability
        Task ToggleAvailabilityAsync(
            string userId);
    }
}