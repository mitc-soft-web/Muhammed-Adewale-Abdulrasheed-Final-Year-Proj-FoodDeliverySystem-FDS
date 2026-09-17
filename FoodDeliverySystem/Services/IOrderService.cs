using FoodDeliverySystem.DTOs.Order;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetCustomerOrdersAsync(
            string customerId);

        Task<OrderListDto> GetAllOrdersAsync(
            string? search,
            string? status,
            string? dateFilter,
            int page,
            int pageSize);

        Task<OrderDto?> GetAdminOrderDetailsAsync(
            int orderId);

        Task<OrderDto?> GetCustomerOrderDetailsAsync(
            int orderId,
            string customerId);

        Task<OrderDto?> GetCustomerTrackingOrderAsync(
            int orderId,
            string customerId);

        Task<List<DeliveryRider>> GetAvailableRidersAsync();

        Task<bool> UpdateStatusAsync(
            int orderId,
            string status);

        Task<(bool Success, string? Error)> UpdateOrderAsync(
            int orderId,
            string status,
            int? deliveryRiderId);

        Task<List<Order>> GetOrdersForExportAsync();

        Task ExpireUnpaidPaymentsAsync();
    }
}