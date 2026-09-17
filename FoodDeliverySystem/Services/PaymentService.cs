using FoodDeliverySystem.DTOs.Payment;
using FoodDeliverySystem.Interfaces;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<Payment> Payments, int TotalItems)> GetPaymentsAsync(
            PaymentFilterDto filter)
        {
            if (filter.Page < 1)
            {
                filter.Page = 1;
            }

            if (filter.PageSize < 1)
            {
                filter.PageSize = 10;
            }

            return await _repository.GetPaymentsAsync(
                filter.Search,
                filter.Status,
                filter.Method,
                filter.Page,
                filter.PageSize);
        }

        public async Task<Payment?> GetPaymentDetailsAsync(int id)
        {
            return await _repository.GetPaymentDetailsAsync(id);
        }

        public async Task<PaymentStatisticsDto> GetStatisticsAsync()
        {
            return new PaymentStatisticsDto
            {
                TotalRevenue =
                    await _repository.GetTotalRevenueAsync(),

                TotalPaid =
                    await _repository.GetTotalPaidAsync(),

                TotalPending =
                    await _repository.GetTotalPendingAsync(),

                TotalFailed =
                    await _repository.GetTotalFailedAsync()
            };
        }

        public async Task<List<Payment>> GetAllPaymentsForExportAsync()
        {
            return await _repository
                .GetAllPaymentsForExportAsync();
        }
    }
}