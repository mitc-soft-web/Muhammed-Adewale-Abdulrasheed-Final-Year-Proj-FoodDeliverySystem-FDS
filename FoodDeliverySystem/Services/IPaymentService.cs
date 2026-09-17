using FoodDeliverySystem.DTOs.Payment;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Interfaces
{
    public interface IPaymentService
    {
        Task<(List<Payment> Payments, int TotalItems)> GetPaymentsAsync(
            PaymentFilterDto filter);

        Task<Payment?> GetPaymentDetailsAsync(int id);

        Task<PaymentStatisticsDto> GetStatisticsAsync();

        Task<List<Payment>> GetAllPaymentsForExportAsync();
    }
}