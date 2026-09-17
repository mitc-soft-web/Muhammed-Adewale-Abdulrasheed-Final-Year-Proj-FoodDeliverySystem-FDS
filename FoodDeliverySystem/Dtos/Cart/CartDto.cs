namespace FoodDeliverySystem.Dtos.Cart
{
    public class CartDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public List<CartItemDto> CartItems { get; set; }
            = new();
    }
}