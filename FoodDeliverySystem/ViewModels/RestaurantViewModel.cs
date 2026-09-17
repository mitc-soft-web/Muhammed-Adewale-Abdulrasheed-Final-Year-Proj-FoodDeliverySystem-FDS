using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class RestaurantViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsOpen { get; set; } = true;

        // Upload Logo
        public IFormFile? LogoFile { get; set; }

        // Upload Cover
        public IFormFile? CoverImageFile { get; set; }

        // Saved paths
        public string? Logo { get; set; }

        public string? CoverImage { get; set; }
    }
}