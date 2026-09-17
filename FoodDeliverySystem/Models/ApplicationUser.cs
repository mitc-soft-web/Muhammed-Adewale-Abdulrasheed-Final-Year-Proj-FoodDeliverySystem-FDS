using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? ProfilePicture { get; set; }

        public string? Role { get; set; }
    }
}