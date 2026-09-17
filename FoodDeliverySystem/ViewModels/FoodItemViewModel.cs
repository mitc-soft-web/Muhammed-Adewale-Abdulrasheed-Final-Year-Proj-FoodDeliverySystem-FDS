using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class FoodItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; }

        [Required]
        public int FoodCategoryId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        // Uploaded image
        public IFormFile? ImageFile { get; set; }

        // Stored image path
        public string? ImageUrl { get; set; }
    }
}
