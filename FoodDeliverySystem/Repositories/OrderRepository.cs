using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetCustomerOrdersAsync(
            string customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync(
            string? search,
            string? status,
            string? dateFilter,
            int page,
            int pageSize)
        {
            var query = BuildAdminQuery(
                search,
                status,
                dateFilter);

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalOrdersCountAsync(
            string? search,
            string? status,
            string? dateFilter)
        {
            var query = BuildAdminQuery(
                search,
                status,
                dateFilter);

            return await query.CountAsync();
        }

        private IQueryable<Order> BuildAdminQuery(
            string? search,
            string? status,
            string? dateFilter)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    (o.Customer != null &&
                     o.Customer.FullName.Contains(search)) ||
                    (o.Customer != null &&
                     o.Customer.PhoneNumber != null &&
                     o.Customer.PhoneNumber.Contains(search)) ||
                    (o.DeliveryRider != null &&
                     o.DeliveryRider.User != null &&
                     o.DeliveryRider.User.FullName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o =>
                    o.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(dateFilter))
            {
                var today = DateTime.Today;

                switch (dateFilter)
                {
                    case "Today":

                        query = query.Where(o =>
                            o.OrderDate.Date == today);

                        break;

                    case "Last7Days":

                        query = query.Where(o =>
                            o.OrderDate >= today.AddDays(-7));

                        break;

                    case "ThisMonth":

                        query = query.Where(o =>
                            o.OrderDate.Month == today.Month &&
                            o.OrderDate.Year == today.Year);

                        break;

                    case "ThisYear":

                        query = query.Where(o =>
                            o.OrderDate.Year == today.Year);

                        break;
                }
            }

            return query;
        }

        public async Task<Order?> GetAdminOrderDetailsAsync(
        int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(f => f.Restaurant)
                            .ThenInclude(r => r.Owner)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(f => f.FoodCategory)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)

                .FirstOrDefaultAsync(o =>
                    o.Id == orderId);
        }

        public async Task<Order?> GetCustomerOrderDetailsAsync(
        int orderId,
        string customerId)
        {
            return await _context.Orders

                .Include(o => o.Customer)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(f => f.Restaurant)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(f => f.FoodCategory)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)

                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.CustomerId == customerId);
        }

        public async Task<Order?> GetCustomerTrackingOrderAsync(
            int orderId,
            string customerId)
        {
            return await _context.Orders
                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)

                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.CustomerId == customerId);
        }

        public async Task<List<DeliveryRider>> GetAvailableRidersAsync()
        {
            return await _context.DeliveryRiders
                .Include(r => r.User)
                .Where(r =>
                    r.Status == "Approved" &&
                    r.IsAvailable)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersForExportAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task ExpireUnpaidPaymentsAsync()
        {
            var expiredPayments = await _context.Payments
                .Include(p => p.Order)
                .Where(p =>
                    p.PaymentStatus == "Awaiting Payment" &&
                    !p.IsExpired &&
                    p.ExpiresAt <= DateTime.Now)
                .ToListAsync();

            foreach (var payment in expiredPayments)
            {
                payment.PaymentStatus = "Payment Failed";
                payment.IsExpired = true;

                if (payment.Order != null)
                {
                    payment.Order.Status = "Payment Failed";
                }
            }

            if (expiredPayments.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}