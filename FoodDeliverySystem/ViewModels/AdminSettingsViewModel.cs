using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class AdminSettingsViewModel
    {
        // =========================
        // ACCOUNT INFORMATION
        // =========================

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }


        // =========================
        // APPEARANCE
        // =========================

        public string Theme { get; set; } = "Light";

        public bool SidebarCollapsed { get; set; }

        public bool RememberSidebarState { get; set; }


        // =========================
        // NOTIFICATIONS
        // =========================

        public bool NotificationsEnabled { get; set; }

        public bool NewOrderNotifications { get; set; }

        public bool RiderApplicationNotifications { get; set; }

        public bool RiderStatusNotifications { get; set; }

        public bool PaymentNotifications { get; set; }

        public bool RestaurantNotifications { get; set; }

        public bool SystemNotifications { get; set; }


        // =========================
        // REGIONAL
        // =========================

        public string Language { get; set; } = "English";

        public string Currency { get; set; } = "NGN";

        public string DateFormat { get; set; } = "dd MMM yyyy";

        public string TimeFormat { get; set; } = "12";


        // =========================
        // DASHBOARD
        // =========================

        public string DefaultDashboard { get; set; } = "Overview";

        public bool ShowRestaurantCard { get; set; }

        public bool ShowCategoryCard { get; set; }

        public bool ShowFoodItemCard { get; set; }

        public bool ShowOrderCard { get; set; }

        public bool ShowRiderCard { get; set; }

        public bool ShowRevenueCard { get; set; }

        public bool AutoRefreshDashboard { get; set; }


        // =========================
        // PRIVACY
        // =========================

        public bool ActivityTrackingEnabled { get; set; }


        // =========================
        // LOGIN
        // =========================
        public bool RememberLogin { get; set; }
    }
}