using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetCustomerOrdersAsync(
            string customerId);

        Task<List<Order>> GetAllOrdersAsync(
            string? search,
            string? status,
            string? dateFilter,
            int page,
            int pageSize);

        Task<int> GetTotalOrdersCountAsync(
            string? search,
            string? status,
            string? dateFilter);

        Task<Order?> GetAdminOrderDetailsAsync(int orderId);

        Task<Order?> GetCustomerOrderDetailsAsync(
            int orderId,
            string customerId);

        Task<Order?> GetCustomerTrackingOrderAsync(
            int orderId,
            string customerId);

        Task<List<DeliveryRider>> GetAvailableRidersAsync();

        Task<Order?> GetByIdAsync(int orderId);

        Task UpdateAsync(Order order);

        Task<List<Order>> GetOrdersForExportAsync();

        Task ExpireUnpaidPaymentsAsync();
    }
}