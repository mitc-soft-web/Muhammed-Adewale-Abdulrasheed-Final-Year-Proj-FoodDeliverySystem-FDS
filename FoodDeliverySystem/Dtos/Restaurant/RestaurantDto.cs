namespace FoodDeliverySystem.Dtos
{
    public class RestaurantDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public string? CoverImage { get; set; }

        public bool IsOpen { get; set; }

        public List<FoodItemDto> FoodItems { get; set; } = new();
    }
}