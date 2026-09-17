using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class FoodItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        // Foreign Key to Restaurant
        public int RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }

        // Foreign Key to Food Category
        public int FoodCategoryId { get; set; }

        public FoodCategory? FoodCategory { get; set; }
    }
}