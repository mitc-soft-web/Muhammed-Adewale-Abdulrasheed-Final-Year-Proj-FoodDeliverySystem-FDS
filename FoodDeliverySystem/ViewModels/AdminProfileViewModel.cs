using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class AdminProfileViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }
    }
}