using FoodDeliverySystem.Dtos;

namespace FoodDeliverySystem.DTOs.Home
{
    public class HomeDto
    {
        public List<HomeFoodItemDto> FoodItems { get; set; } = new();

        public List<HomeRestaurantDto> Restaurants { get; set; } = new();
    }

    public class HomeFoodItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsFeatured { get; set; }

        public int RestaurantId { get; set; }

        public string? RestaurantName { get; set; }

        public bool RestaurantIsOpen { get; set; }

        public int FoodCategoryId { get; set; }

        public string? FoodCategoryName { get; set; }
    }

    public class HomeRestaurantDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public string? CoverImage { get; set; }

        public bool IsOpen { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? OwnerId { get; set; }
    }
}