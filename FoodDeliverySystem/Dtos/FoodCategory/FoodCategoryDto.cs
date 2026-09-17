namespace FoodDeliverySystem.Dtos.FoodCategory
{
    public class FoodCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int FoodItemCount { get; set; }
    }
}