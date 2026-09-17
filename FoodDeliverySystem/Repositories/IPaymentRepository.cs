using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Interfaces
{
    public interface IPaymentRepository
    {
        Task<(List<Payment> Payments, int TotalItems)> GetPaymentsAsync(
            string? search,
            string? status,
            string? method,
            int page,
            int pageSize);

        Task<Payment?> GetPaymentDetailsAsync(int id);

        Task<decimal> GetTotalRevenueAsync();

        Task<int> GetTotalPaidAsync();

        Task<int> GetTotalPendingAsync();

        Task<int> GetTotalFailedAsync();

        Task<List<Payment>> GetAllPaymentsForExportAsync();
    }
}