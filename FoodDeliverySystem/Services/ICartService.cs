using FoodDeliverySystem.Dtos.Cart;

namespace FoodDeliverySystem.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(string userId);

        Task AddToCartAsync(
            string userId,
            int foodId,
            int quantity);

        Task IncreaseQuantityAsync(
            string userId,
            int cartItemId);

        Task DecreaseQuantityAsync(
            string userId,
            int cartItemId);

        Task RemoveFromCartAsync(
            string userId,
            int cartItemId);
    }
}