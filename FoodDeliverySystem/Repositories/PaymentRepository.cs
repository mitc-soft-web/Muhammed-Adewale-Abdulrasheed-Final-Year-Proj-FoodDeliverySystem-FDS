using FoodDeliverySystem.Data;
using FoodDeliverySystem.Interfaces;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Payment> Payments, int TotalItems)> GetPaymentsAsync(
            string? search,
            string? status,
            string? method,
            int page,
            int pageSize)
        {
            var query = _context.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Id.ToString().Contains(search) ||
                    p.OrderId.ToString().Contains(search) ||
                    EF.Functions.Like(
                        p.PaymentMethod,
                        $"%{search}%") ||
                    EF.Functions.Like(
                        p.Order.Customer.FullName,
                        $"%{search}%"));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p =>
                    p.PaymentStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(method))
            {
                query = query.Where(p =>
                    p.PaymentMethod == method);
            }

            var totalItems = await query.CountAsync();

            var payments = await query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (payments, totalItems);
        }

        public async Task<Payment?> GetPaymentDetailsAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Customer)

                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.FoodItem)
                            .ThenInclude(f => f.Restaurant)

                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.FoodItem)
                            .ThenInclude(f => f.FoodCategory)

                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Payments
                .Where(p => p.PaymentStatus == "Paid")
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetTotalPaidAsync()
        {
            return await _context.Payments
                .CountAsync(p =>
                    p.PaymentStatus == "Paid");
        }

        public async Task<int> GetTotalPendingAsync()
        {
            return await _context.Payments
                .CountAsync(p =>
                    p.PaymentStatus == "Pending Collection" ||
                    p.PaymentStatus == "Awaiting Payment");
        }

        public async Task<int> GetTotalFailedAsync()
        {
            return await _context.Payments
                .CountAsync(p =>
                    p.PaymentStatus == "Payment Failed");
        }

        public async Task<List<Payment>> GetAllPaymentsForExportAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Customer)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}