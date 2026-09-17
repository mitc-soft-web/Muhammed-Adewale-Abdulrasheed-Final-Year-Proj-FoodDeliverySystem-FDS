using FoodDeliverySystem.DTOs.Admin;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace FoodDeliverySystem.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(
        IAdminRepository repository,
        UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }


        // =========================================================
        // ADMIN DASHBOARD
        // =========================================================

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            // Expire unpaid online payments first.
            await _repository.ExpireUnpaidPaymentsAsync();

            var dashboard = new AdminDashboardDto
            {
                Restaurants =
                    await _repository.GetRestaurantCountAsync(),

                Categories =
                    await _repository.GetCategoryCountAsync(),

                FoodItems =
                    await _repository.GetFoodItemCountAsync(),

                Orders =
                    await _repository.GetOrderCountAsync(),

                NewOrders =
                    await _repository.GetPendingOrderCountAsync(),

                SuccessfulPayments =
                    await _repository.GetSuccessfulPaymentCountAsync(),

                FailedPayments =
                    await _repository.GetFailedPaymentCountAsync(),

                AvailableRiders =
                    await _repository.GetAvailableRiderCountAsync(),

                TodayOrders =
                    await _repository.GetTodayOrderCountAsync(),

                TodayRevenue =
                    await _repository.GetTodayRevenueAsync(),

                OpenRestaurants =
                    await _repository.GetOpenRestaurantCountAsync(),

                RecentOrders =
                    await _repository.GetRecentOrdersAsync(5),

                TopFoods =
                    await _repository.GetTopFoodsAsync(5),

                PendingOrders =
                    await _repository.GetPendingOrderCountAsync(),

                PreparingOrders =
                    await _repository.GetPreparingOrderCountAsync(),

                DeliveredOrders =
                    await _repository.GetDeliveredOrderCountAsync(),

                CancelledOrders =
                    await _repository.GetCancelledOrderCountAsync()
            };


            // =====================================================
            // RECENT ACTIVITY
            // =====================================================

            var activities = new List<AdminActivityDto>();

            var orders =
                await _repository.GetRecentOrdersForActivityAsync(5);

            foreach (var order in orders)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-bag-check-fill",
                    Type = "Orders",
                    Title = "New Order",
                    Description = $"Order #{order.Id} was placed.",
                    Time = order.OrderDate
                });
            }


            var payments =
                await _repository.GetRecentSuccessfulPaymentsAsync(5);

            foreach (var payment in payments)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-credit-card-fill",
                    Type = "Payments",
                    Title = "Payment Received",
                    Description =
                        $"Payment of ₦{payment.Amount:N0} completed.",
                    Time = payment.PaymentDate
                });
            }


            var restaurants =
                await _repository.GetRecentRestaurantsAsync(5);

            foreach (var restaurant in restaurants)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-shop",
                    Type = "Restaurants",
                    Title = "Restaurant Added",
                    Description =
                        $"{restaurant.Name} joined Food Xpress.",
                    Time = restaurant.CreatedAt
                });
            }


            var riders =
                await _repository.GetRecentRidersAsync(5);

            foreach (var rider in riders)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-bicycle",
                    Type = "Riders",
                    Title = "New Rider",
                    Description =
                        $"{rider.User.FullName} registered as a rider.",
                    Time = rider.CreatedAt
                });
            }


            dashboard.RecentActivities = activities
                .OrderByDescending(x => x.Time)
                .Take(10)
                .ToList();


            // =====================================================
            // SALES CHART — LAST 7 DAYS
            // =====================================================

            for (int i = 6; i >= 0; i--)
            {
                var day = DateTime.Today.AddDays(-i);

                dashboard.SalesLabels.Add(
                    day.ToString("ddd"));

                dashboard.SalesRevenue.Add(
                    await _repository.GetRevenueForDayAsync(day));

                dashboard.OrderChart.Add(
                    await _repository.GetOrdersForDayAsync(day));

                dashboard.PaymentChart.Add(
                    await _repository
                        .GetSuccessfulPaymentsForDayAsync(day));
            }

            return dashboard;
        }


        // =========================================================
        // ACTIVITY LOG
        // =========================================================

        public async Task<List<AdminActivityDto>> GetActivityAsync()
        {
            var activities = new List<AdminActivityDto>();


            var orders =
                await _repository.GetRecentOrdersForActivityAsync(100);

            foreach (var order in orders)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-bag-check-fill",
                    Type = "Orders",
                    Title = "New Order",
                    Description =
                        $"Order #{order.Id} was placed.",
                    Time = order.OrderDate
                });
            }


            var payments =
                await _repository.GetRecentSuccessfulPaymentsAsync(100);

            foreach (var payment in payments)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-credit-card-fill",
                    Type = "Payments",
                    Title = "Payment Received",
                    Description =
                        $"Payment of ₦{payment.Amount:N0} completed.",
                    Time = payment.PaymentDate
                });
            }


            var restaurants =
                await _repository.GetRecentRestaurantsAsync(100);

            foreach (var restaurant in restaurants)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-shop",
                    Type = "Restaurants",
                    Title = "Restaurant Added",
                    Description =
                        $"{restaurant.Name} joined Food Xpress.",
                    Time = restaurant.CreatedAt
                });
            }


            var riders =
                await _repository.GetRecentRidersAsync(100);

            foreach (var rider in riders)
            {
                activities.Add(new AdminActivityDto
                {
                    Icon = "bi bi-bicycle",
                    Type = "Riders",
                    Title = "New Rider",
                    Description =
                        $"{rider.User.FullName} registered as a rider.",
                    Time = rider.CreatedAt
                });
            }


            return activities
                .OrderByDescending(x => x.Time)
                .ToList();
        }


        // =========================================================
        // SETTINGS
        // =========================================================

        public async Task<AdminSettingsDto> GetSettingsAsync(
            ApplicationUser user)
        {
            var settings =
                await _repository.GetOrCreateSettingsAsync(user.Id);

            return new AdminSettingsDto
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,

                Theme = settings.Theme,
                SidebarCollapsed = settings.SidebarCollapsed,
                RememberSidebarState = settings.RememberSidebarState,

                NotificationsEnabled = settings.NotificationsEnabled,
                NewOrderNotifications = settings.NewOrderNotifications,
                RiderApplicationNotifications =
                    settings.RiderApplicationNotifications,
                RiderStatusNotifications =
                    settings.RiderStatusNotifications,
                PaymentNotifications =
                    settings.PaymentNotifications,
                RestaurantNotifications =
                    settings.RestaurantNotifications,
                SystemNotifications =
                    settings.SystemNotifications,

                Language = settings.Language,
                Currency = settings.Currency,
                DateFormat = settings.DateFormat,
                TimeFormat = settings.TimeFormat,

                DefaultDashboard = settings.DefaultDashboard,
                ShowRestaurantCard = settings.ShowRestaurantCard,
                ShowCategoryCard = settings.ShowCategoryCard,
                ShowFoodItemCard = settings.ShowFoodItemCard,
                ShowOrderCard = settings.ShowOrderCard,
                ShowRiderCard = settings.ShowRiderCard,
                ShowRevenueCard = settings.ShowRevenueCard,
                AutoRefreshDashboard = settings.AutoRefreshDashboard,

                ActivityTrackingEnabled =
                    settings.ActivityTrackingEnabled,

                RememberLogin =
                    settings.RememberLogin
            };
        }


        public async Task SaveSettingsAsync(
            ApplicationUser user,
            AdminSettingsDto dto)
        {
            var settings =
                await _repository.GetOrCreateSettingsAsync(user.Id);

            settings.Theme = dto.Theme;
            settings.SidebarCollapsed = dto.SidebarCollapsed;
            settings.RememberSidebarState = dto.RememberSidebarState;

            settings.NotificationsEnabled =
                dto.NotificationsEnabled;

            settings.NewOrderNotifications =
                dto.NewOrderNotifications;

            settings.RiderApplicationNotifications =
                dto.RiderApplicationNotifications;

            settings.RiderStatusNotifications =
                dto.RiderStatusNotifications;

            settings.PaymentNotifications =
                dto.PaymentNotifications;

            settings.RestaurantNotifications =
                dto.RestaurantNotifications;

            settings.SystemNotifications =
                dto.SystemNotifications;

            settings.Language = dto.Language;
            settings.Currency = dto.Currency;
            settings.DateFormat = dto.DateFormat;
            settings.TimeFormat = dto.TimeFormat;

            settings.DefaultDashboard =
                dto.DefaultDashboard;

            settings.ShowRestaurantCard =
                dto.ShowRestaurantCard;

            settings.ShowCategoryCard =
                dto.ShowCategoryCard;

            settings.ShowFoodItemCard =
                dto.ShowFoodItemCard;

            settings.ShowOrderCard =
                dto.ShowOrderCard;

            settings.ShowRiderCard =
                dto.ShowRiderCard;

            settings.ShowRevenueCard =
                dto.ShowRevenueCard;

            settings.AutoRefreshDashboard =
                dto.AutoRefreshDashboard;

            settings.ActivityTrackingEnabled =
                dto.ActivityTrackingEnabled;

            settings.RememberLogin =
                dto.RememberLogin;

            settings.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveSettingsAsync(settings);
        }


        // =========================================================
        // PROFILE
        // =========================================================

        public Task<AdminProfileDto> GetProfileAsync(
            ApplicationUser user)
        {
            return Task.FromResult(
                new AdminProfileDto
                {
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Gender = user.Gender
                });
        }


        public async Task<(bool Success, string? Error)>
            UpdateProfileAsync(
                ApplicationUser user,
                AdminProfileDto dto)
        {
            if (!string.Equals(
                    user.Email,
                    dto.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                var existingUser =
                    await _repository.FindUserByEmailAsync(dto.Email);

                if (existingUser != null &&
                    existingUser.Id != user.Id)
                {
                    return (
                        false,
                        "This email address is already in use.");
                }

                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.NormalizedEmail =
                    dto.Email.ToUpperInvariant();
                user.NormalizedUserName =
                    dto.Email.ToUpperInvariant();

                user.EmailConfirmed = true;
            }

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Gender = dto.Gender;

            await _repository.UpdateUserAsync(user);

            return (true, null);
        }


        // =========================================================
        // NOTIFICATIONS
        // =========================================================

        public Task<List<Notification>> GetNotificationsAsync(
            string filter,
            string search,
            int page,
            int pageSize)
        {
            return _repository.GetNotificationsAsync(
                filter,
                search,
                page,
                pageSize);
        }


        public Task<int> GetNotificationCountAsync(
            string filter,
            string search)
        {
            return _repository.GetNotificationCountAsync(
                filter,
                search);
        }


        public Task<int> GetUnreadNotificationCountAsync()
        {
            return _repository.GetUnreadNotificationCountAsync();
        }


        public Task MarkAllNotificationsReadAsync()
        {
            return _repository.MarkAllNotificationsReadAsync();
        }


        public async Task<(bool Success, IEnumerable<string> Errors)>
        ChangePasswordAsync(
        ApplicationUser user,
        string currentPassword,
        string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

            if (!result.Succeeded)
            {
                return (
                    false,
                    result.Errors.Select(e => e.Description)
                );
            }

            return (true, Enumerable.Empty<string>());
        }


        public async Task<bool> UpdateProfilePhotoAsync(
        ApplicationUser user,
        IFormFile photo)
        {
            if (photo == null || photo.Length <= 0)
                return false;

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "profile");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName =
                Guid.NewGuid() +
                Path.GetExtension(photo.FileName);

            var filePath =
                Path.Combine(uploadsFolder, fileName);

            using (var stream =
                   new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            user.ProfilePicture =
                "/uploads/profile/" + fileName;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}