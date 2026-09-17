using FoodDeliverySystem.DTOs.Admin;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAdminService adminService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboard =
                await _adminService.GetDashboardAsync();

            ViewBag.Restaurants =
                dashboard.Restaurants;

            ViewBag.Categories =
                dashboard.Categories;

            ViewBag.FoodItems =
                dashboard.FoodItems;

            ViewBag.Orders =
                dashboard.Orders;

            ViewBag.NewOrders =
                dashboard.NewOrders;

            ViewBag.SuccessfulPayments =
                dashboard.SuccessfulPayments;

            ViewBag.FailedPayments =
                dashboard.FailedPayments;

            ViewBag.AvailableRiders =
                dashboard.AvailableRiders;

            ViewBag.TodayOrders =
                dashboard.TodayOrders;

            ViewBag.TodayRevenue =
                dashboard.TodayRevenue;

            ViewBag.OpenRestaurants =
                dashboard.OpenRestaurants;

            ViewBag.RecentOrders =
                dashboard.RecentOrders;

            ViewBag.RecentActivities =
                dashboard.RecentActivities;

            ViewBag.TopFoods =
                dashboard.TopFoods;

            ViewBag.SalesLabels =
                dashboard.SalesLabels;

            ViewBag.SalesRevenue =
                dashboard.SalesRevenue;

            ViewBag.OrderChart =
                dashboard.OrderChart;

            ViewBag.PaymentChart =
                dashboard.PaymentChart;

            ViewBag.PendingOrders =
                dashboard.PendingOrders;

            ViewBag.PreparingOrders =
                dashboard.PreparingOrders;

            ViewBag.DeliveredOrders =
                dashboard.DeliveredOrders;

            ViewBag.CancelledOrders =
                dashboard.CancelledOrders;

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            return View(user);
        }


        // GET: Admin Settings
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var dto = await _adminService.GetSettingsAsync(user);

            var model = new AdminSettingsViewModel
            {
                FullName = dto.FullName,
                Email = dto.Email,
                EmailConfirmed = dto.EmailConfirmed,

                Theme = dto.Theme,
                SidebarCollapsed = dto.SidebarCollapsed,
                RememberSidebarState = dto.RememberSidebarState,

                NotificationsEnabled = dto.NotificationsEnabled,
                NewOrderNotifications = dto.NewOrderNotifications,
                RiderApplicationNotifications =
                    dto.RiderApplicationNotifications,
                RiderStatusNotifications =
                    dto.RiderStatusNotifications,
                PaymentNotifications =
                    dto.PaymentNotifications,
                RestaurantNotifications =
                    dto.RestaurantNotifications,
                SystemNotifications =
                    dto.SystemNotifications,

                Language = dto.Language,
                Currency = dto.Currency,
                DateFormat = dto.DateFormat,
                TimeFormat = dto.TimeFormat,

                DefaultDashboard = dto.DefaultDashboard,
                ShowRestaurantCard = dto.ShowRestaurantCard,
                ShowCategoryCard = dto.ShowCategoryCard,
                ShowFoodItemCard = dto.ShowFoodItemCard,
                ShowOrderCard = dto.ShowOrderCard,
                ShowRiderCard = dto.ShowRiderCard,
                ShowRevenueCard = dto.ShowRevenueCard,
                AutoRefreshDashboard = dto.AutoRefreshDashboard,

                ActivityTrackingEnabled =
                    dto.ActivityTrackingEnabled,

                RememberLogin = dto.RememberLogin
            };

            return View(model);
        }


        // POST: Admin Settings
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(
    AdminSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var dto = new AdminSettingsDto
            {
                Theme = model.Theme,
                SidebarCollapsed = model.SidebarCollapsed,
                RememberSidebarState = model.RememberSidebarState,

                NotificationsEnabled = model.NotificationsEnabled,
                NewOrderNotifications = model.NewOrderNotifications,
                RiderApplicationNotifications =
                    model.RiderApplicationNotifications,
                RiderStatusNotifications =
                    model.RiderStatusNotifications,
                PaymentNotifications =
                    model.PaymentNotifications,
                RestaurantNotifications =
                    model.RestaurantNotifications,
                SystemNotifications =
                    model.SystemNotifications,

                Language = model.Language,
                Currency = model.Currency,
                DateFormat = model.DateFormat,
                TimeFormat = model.TimeFormat,

                DefaultDashboard = model.DefaultDashboard,
                ShowRestaurantCard = model.ShowRestaurantCard,
                ShowCategoryCard = model.ShowCategoryCard,
                ShowFoodItemCard = model.ShowFoodItemCard,
                ShowOrderCard = model.ShowOrderCard,
                ShowRiderCard = model.ShowRiderCard,
                ShowRevenueCard = model.ShowRevenueCard,
                AutoRefreshDashboard = model.AutoRefreshDashboard,

                ActivityTrackingEnabled =
                    model.ActivityTrackingEnabled,

                RememberLogin = model.RememberLogin
            };

            await _adminService.SaveSettingsAsync(user, dto);

            TempData["Success"] = "Settings saved successfully.";

            return RedirectToAction(nameof(Settings));
        }

        // GET
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var dto = await _adminService.GetProfileAsync(user);

            var model = new AdminProfileViewModel
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender
            };

            return View(model);
        }

        // POST
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(
    AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var dto = new AdminProfileDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Gender = model.Gender
            };

            var result =
                await _adminService.UpdateProfileAsync(user, dto);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    "Email",
                    result.Error ?? "Unable to update profile.");

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] =
                "Profile updated successfully.";

            return RedirectToAction(nameof(Profile));
        }


        // GET - Change Admin Password
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        // POST - Change Admin Password
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var result =
                await _adminService.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] =
                "Your password has been changed successfully.";

            return RedirectToAction(nameof(Profile));
        }


        //=======================================
        // CHANGE PROFILE PHOTO
        //=======================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ChangePhoto()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoto(IFormFile photo)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] =
                    "Please select a profile picture.";

                return RedirectToAction(nameof(ChangePhoto));
            }

            var updated =
                await _adminService.UpdateProfilePhotoAsync(
                    user,
                    photo);

            if (!updated)
            {
                TempData["Error"] =
                    "We couldn't update your profile picture.";

                return RedirectToAction(nameof(ChangePhoto));
            }

            TempData["Success"] =
                "Profile picture updated successfully.";

            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> ActivityLog(int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var activities =
                await _adminService.GetActivityAsync();

            var totalItems =
                activities.Count;

            var totalPages =
                (int)Math.Ceiling(
                    totalItems / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            ViewBag.CurrentPage =
                page;

            ViewBag.TotalPages =
                totalPages;

            ViewBag.RecentActivities =
                activities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            ViewBag.UnreadCount =
                await _adminService
                    .GetUnreadNotificationCountAsync();

            return View();
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Notifications(
          string filter = "All",
          string search = "",
          int page = 1)
        {
            const int pageSize = 20;

            if (page < 1)
            {
                page = 1;
            }

            var notifications =
                await _adminService.GetNotificationsAsync(
                    filter,
                    search,
                    page,
                    pageSize);

            var totalCount =
                await _adminService.GetNotificationCountAsync(
                    filter,
                    search);

            ViewBag.Filter = filter;
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    totalCount / (double)pageSize);

            return View(notifications);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllNotificationsRead()
        {
            await _adminService.MarkAllNotificationsReadAsync();

            return RedirectToAction(
                nameof(Notifications));
        }
    }
}