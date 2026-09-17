using FoodDeliverySystem.Data;
using FoodDeliverySystem.DTOs.Admin;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // PAYMENT EXPIRATION
        // =========================================================

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


        // =========================================================
        // COUNTS
        // =========================================================

        public Task<int> GetRestaurantCountAsync()
        {
            return _context.Restaurants.CountAsync();
        }

        public Task<int> GetCategoryCountAsync()
        {
            return _context.FoodCategories.CountAsync();
        }

        public Task<int> GetFoodItemCountAsync()
        {
            return _context.FoodItems.CountAsync();
        }

        public Task<int> GetOrderCountAsync()
        {
            return _context.Orders.CountAsync();
        }

        public Task<int> GetPendingOrderCountAsync()
        {
            return _context.Orders
                .CountAsync(o => o.Status == "Pending");
        }

        public Task<int> GetSuccessfulPaymentCountAsync()
        {
            return _context.Payments
                .CountAsync(p => p.PaymentStatus == "Paid");
        }

        public Task<int> GetFailedPaymentCountAsync()
        {
            return _context.Payments
                .CountAsync(p => p.PaymentStatus == "Payment Failed");
        }

        public Task<int> GetAvailableRiderCountAsync()
        {
            return _context.DeliveryRiders
                .CountAsync(r => r.IsAvailable);
        }


        // =========================================================
        // TODAY
        // =========================================================

        public Task<int> GetTodayOrderCountAsync()
        {
            return _context.Orders
                .CountAsync(o => o.OrderDate.Date == DateTime.Today);
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            return await _context.Payments
                .Where(p =>
                    p.PaymentStatus == "Paid" &&
                    p.PaymentDate.Date == DateTime.Today)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
        }

        public Task<int> GetOpenRestaurantCountAsync()
        {
            return _context.Restaurants
                .CountAsync(r => r.IsOpen);
        }


        // =========================================================
        // RECENT DATA
        // =========================================================

        public Task<List<Order>> GetRecentOrdersAsync(int count)
        {
            return _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public Task<List<Order>> GetRecentOrdersForActivityAsync(int count)
        {
            return _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public Task<List<Payment>> GetRecentSuccessfulPaymentsAsync(int count)
        {
            return _context.Payments
                .Where(p => p.PaymentStatus == "Paid")
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .ToListAsync();
        }

        public Task<List<Restaurant>> GetRecentRestaurantsAsync(int count)
        {
            return _context.Restaurants
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public Task<List<DeliveryRider>> GetRecentRidersAsync(int count)
        {
            return _context.DeliveryRiders
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }


        // =========================================================
        // TOP FOODS
        // =========================================================

        public Task<List<TopFoodDto>> GetTopFoodsAsync(int count)
        {
            return _context.OrderItems
                .Include(o => o.FoodItem)
                .GroupBy(o => new
                {
                    o.FoodItemId,
                    o.FoodItem.Name
                })
                .Select(g => new TopFoodDto
                {
                    Food = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(count)
                .ToListAsync();
        }


        // =========================================================
        // CHART DATA
        // =========================================================

        public async Task<decimal> GetRevenueForDayAsync(DateTime day)
        {
            return await _context.Payments
                .Where(p =>
                    p.PaymentStatus == "Paid" &&
                    p.PaymentDate.Date == day.Date)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
        }

        public Task<int> GetOrdersForDayAsync(DateTime day)
        {
            return _context.Orders
                .CountAsync(o => o.OrderDate.Date == day.Date);
        }

        public Task<int> GetSuccessfulPaymentsForDayAsync(DateTime day)
        {
            return _context.Payments
                .CountAsync(p =>
                    p.PaymentStatus == "Paid" &&
                    p.PaymentDate.Date == day.Date);
        }


        // =========================================================
        // ORDER STATUS
        // =========================================================

        public Task<int> GetPreparingOrderCountAsync()
        {
            return _context.Orders
                .CountAsync(o => o.Status == "Preparing");
        }

        public Task<int> GetDeliveredOrderCountAsync()
        {
            return _context.Orders
                .CountAsync(o => o.Status == "Delivered");
        }

        public Task<int> GetCancelledOrderCountAsync()
        {
            return _context.Orders
                .CountAsync(o => o.Status == "Cancelled");
        }

        // =========================================================
        // ADMIN SETTINGS
        // =========================================================

        public async Task<AdminSettings?> GetSettingsAsync(string userId)
        {
            return await _context.AdminSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }


        public async Task<AdminSettings> GetOrCreateSettingsAsync(string userId)
        {
            var settings = await _context.AdminSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings != null)
                return settings;

            settings = new AdminSettings
            {
                UserId = userId
            };

            _context.AdminSettings.Add(settings);

            await _context.SaveChangesAsync();

            return settings;
        }


        public async Task SaveSettingsAsync(AdminSettings settings)
        {
            _context.AdminSettings.Update(settings);

            await _context.SaveChangesAsync();
        }


        // =========================================================
        // ADMIN USER
        // =========================================================

        public Task<ApplicationUser?> GetUserAsync(string userId)
        {
            return _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }


        public Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task UpdateUserAsync(ApplicationUser user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }


        // =========================================================
        // NOTIFICATIONS
        // =========================================================

        public async Task<List<Notification>> GetNotificationsAsync(
            string filter,
            string search,
            int page,
            int pageSize)
        {
            var query = _context.Notifications
                .AsQueryable();

            if (filter != "All")
            {
                if (filter == "Unread")
                {
                    query = query.Where(n => !n.IsRead);
                }
                else
                {
                    query = query.Where(n => n.Type == filter);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(n =>
                    n.Title.Contains(search) ||
                    n.Message.Contains(search));
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<int> GetNotificationCountAsync(
            string filter,
            string search)
        {
            var query = _context.Notifications
                .AsQueryable();

            if (filter != "All")
            {
                if (filter == "Unread")
                {
                    query = query.Where(n => !n.IsRead);
                }
                else
                {
                    query = query.Where(n => n.Type == filter);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(n =>
                    n.Title.Contains(search) ||
                    n.Message.Contains(search));
            }

            return await query.CountAsync();
        }


        public Task<int> GetUnreadNotificationCountAsync()
        {
            return _context.Notifications
                .CountAsync(x => !x.IsRead);
        }


        public async Task MarkAllNotificationsReadAsync()
        {
            var notifications = await _context.Notifications
                .Where(x => !x.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}