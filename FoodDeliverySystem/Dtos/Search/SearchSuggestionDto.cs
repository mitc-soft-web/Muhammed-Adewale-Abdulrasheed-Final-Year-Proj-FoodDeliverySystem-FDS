namespace FoodDeliverySystem.DTOs.Search
{
    public class SearchSuggestionDto
    {
        public string Type { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;

        public int RestaurantId { get; set; }
    }
}