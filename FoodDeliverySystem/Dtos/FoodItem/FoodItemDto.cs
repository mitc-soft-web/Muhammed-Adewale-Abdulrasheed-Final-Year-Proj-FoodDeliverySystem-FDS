namespace FoodDeliverySystem.Dtos
{
    public class FoodItemDto
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

    public class FoodCategoryOptionDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class RestaurantOptionDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsOpen { get; set; }
    }
}