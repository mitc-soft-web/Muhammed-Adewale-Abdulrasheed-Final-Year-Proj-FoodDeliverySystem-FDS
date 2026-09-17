using FoodDeliverySystem.DTOs.Admin;
using FoodDeliverySystem.Models;
using Microsoft.AspNetCore.Http;

namespace FoodDeliverySystem.Services
{
    public interface IAdminService
    {
        Task<AdminSettingsDto> GetSettingsAsync(ApplicationUser user);

        Task SaveSettingsAsync(
            ApplicationUser user,
            AdminSettingsDto dto);

        Task<AdminProfileDto> GetProfileAsync(
            ApplicationUser user);

        Task<(bool Success, string? Error)> UpdateProfileAsync(
            ApplicationUser user,
            AdminProfileDto dto);

        Task<List<Notification>> GetNotificationsAsync(
            string filter,
            string search,
            int page,
            int pageSize);

        Task<int> GetNotificationCountAsync(
            string filter,
            string search);

        Task<int> GetUnreadNotificationCountAsync();

        Task MarkAllNotificationsReadAsync();


        Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(
        ApplicationUser user,
        string currentPassword,
        string newPassword);

        Task<bool> UpdateProfilePhotoAsync(
            ApplicationUser user,
            IFormFile photo);


        Task<AdminDashboardDto> GetDashboardAsync();

        Task<List<AdminActivityDto>> GetActivityAsync();
    }
}