namespace FoodDeliverySystem.Dtos.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int FoodItemId { get; set; }

        public string FoodItemName { get; set; } = string.Empty;

        public string? FoodItemDescription { get; set; }

        public string? FoodItemImageUrl { get; set; }

        public decimal FoodItemPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice =>
            FoodItemPrice * Quantity;
    }
}