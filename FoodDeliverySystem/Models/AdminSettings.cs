using System.ComponentModel.DataAnnotations;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Models
{
    public class AdminSettings
    {
        public int Id { get; set; }

        // Link settings to the existing ASP.NET Identity user
        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        // =========================
        // APPEARANCE
        // =========================

        // Light, Dark, or System
        public string Theme { get; set; } = "Light";

        // true = sidebar collapsed
        public bool SidebarCollapsed { get; set; } = false;

        // Remember the administrator's sidebar preference
        public bool RememberSidebarState { get; set; } = true;


        // =========================
        // NOTIFICATIONS
        // =========================

        public bool NotificationsEnabled { get; set; } = true;

        public bool NewOrderNotifications { get; set; } = true;

        public bool RiderApplicationNotifications { get; set; } = true;

        public bool RiderStatusNotifications { get; set; } = true;

        public bool PaymentNotifications { get; set; } = true;

        public bool RestaurantNotifications { get; set; } = true;

        public bool SystemNotifications { get; set; } = true;


        // =========================
        // REGIONAL PREFERENCES
        // =========================

        public string Language { get; set; } = "English";

        public string Currency { get; set; } = "NGN";

        public string DateFormat { get; set; } = "dd MMM yyyy";

        public string TimeFormat { get; set; } = "12";


        // =========================
        // DASHBOARD PREFERENCES
        // =========================

        public string DefaultDashboard { get; set; } = "Overview";

        public bool ShowRestaurantCard { get; set; } = true;

        public bool ShowCategoryCard { get; set; } = true;

        public bool ShowFoodItemCard { get; set; } = true;

        public bool ShowOrderCard { get; set; } = true;

        public bool ShowRiderCard { get; set; } = true;

        public bool ShowRevenueCard { get; set; } = true;

        public bool AutoRefreshDashboard { get; set; } = true;


        // =========================
        // PRIVACY
        // =========================

        public bool ActivityTrackingEnabled { get; set; } = true;


        // =========================
        // ACCOUNT SECURITY
        // =========================

        public bool RememberLogin { get; set; } = false;


        // =========================
        // CREATED / UPDATED
        // =========================

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}