using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(string userId);

        Task<Cart?> GetByUserIdWithItemsAsync(string userId);

        Task<CartItem?> GetCartItemAsync(
            string userId,
            int cartItemId);

        Task<CartItem?> GetCartItemByFoodAsync(
            string userId,
            int foodId);

        Task<Cart> CreateCartAsync(string userId);

        Task AddCartItemAsync(CartItem cartItem);

        Task UpdateCartItemAsync(CartItem cartItem);

        Task RemoveCartItemAsync(CartItem cartItem);

        Task SaveChangesAsync();
    }
}