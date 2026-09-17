using FoodDeliverySystem.ViewModels;

namespace FoodDeliverySystem.DTOs.Admin
{
    public class AdminSettingsDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public string Theme { get; set; } = "Light";

        public bool SidebarCollapsed { get; set; }

        public bool RememberSidebarState { get; set; }

        public bool NotificationsEnabled { get; set; }

        public bool NewOrderNotifications { get; set; }

        public bool RiderApplicationNotifications { get; set; }

        public bool RiderStatusNotifications { get; set; }

        public bool PaymentNotifications { get; set; }

        public bool RestaurantNotifications { get; set; }

        public bool SystemNotifications { get; set; }

        public string Language { get; set; } = "English";

        public string Currency { get; set; } = "NGN";

        public string DateFormat { get; set; } = "dd/MM/yyyy";

        public string TimeFormat { get; set; } = "12-hour";

        public string DefaultDashboard { get; set; } = "Dashboard";

        public bool ShowRestaurantCard { get; set; }

        public bool ShowCategoryCard { get; set; }

        public bool ShowFoodItemCard { get; set; }

        public bool ShowOrderCard { get; set; }

        public bool ShowRiderCard { get; set; }

        public bool ShowRevenueCard { get; set; }

        public bool AutoRefreshDashboard { get; set; }

        public bool ActivityTrackingEnabled { get; set; }

        public bool RememberLogin { get; set; }
    }
}