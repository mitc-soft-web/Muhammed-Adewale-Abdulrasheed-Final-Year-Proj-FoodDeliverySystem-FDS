using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.FoodItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemAsync(
            string userId,
            int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.Id == cartItemId &&
                    ci.Cart != null &&
                    ci.Cart.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemByFoodAsync(
            string userId,
            int foodId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci =>
                    ci.FoodItemId == foodId &&
                    ci.Cart != null &&
                    ci.Cart.UserId == userId);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}