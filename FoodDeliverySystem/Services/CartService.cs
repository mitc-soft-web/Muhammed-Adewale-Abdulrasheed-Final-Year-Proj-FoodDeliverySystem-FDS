using FoodDeliverySystem.Dtos.Cart;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDto?> GetCartAsync(string userId)
        {
            var cart = await _cartRepository
                .GetByUserIdWithItemsAsync(userId);

            if (cart == null)
            {
                return null;
            }

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,

                CartItems = cart.CartItems
                    .Select(item => new CartItemDto
                    {
                        Id = item.Id,
                        CartId = item.CartId,
                        FoodItemId = item.FoodItemId,

                        FoodItemName =
                            item.FoodItem?.Name ?? string.Empty,

                        FoodItemDescription =
                            item.FoodItem?.Description,

                        FoodItemImageUrl =
                            item.FoodItem?.ImageUrl,

                        FoodItemPrice =
                            item.FoodItem?.Price ?? 0,

                        Quantity = item.Quantity
                    })
                    .ToList()
            };
        }

        public async Task AddToCartAsync(
            string userId,
            int foodId,
            int quantity)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            var cart = await _cartRepository
                .GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = await _cartRepository
                    .CreateCartAsync(userId);
            }

            var existingItem =
                await _cartRepository.GetCartItemByFoodAsync(
                    userId,
                    foodId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;

                await _cartRepository
                    .UpdateCartItemAsync(existingItem);

                return;
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                FoodItemId = foodId,
                Quantity = quantity
            };

            await _cartRepository
                .AddCartItemAsync(cartItem);
        }

        public async Task IncreaseQuantityAsync(
            string userId,
            int cartItemId)
        {
            var item = await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItemId);

            if (item == null)
            {
                return;
            }

            item.Quantity++;

            await _cartRepository
                .UpdateCartItemAsync(item);
        }

        public async Task DecreaseQuantityAsync(
            string userId,
            int cartItemId)
        {
            var item = await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItemId);

            if (item == null)
            {
                return;
            }

            item.Quantity--;

            if (item.Quantity <= 0)
            {
                await _cartRepository
                    .RemoveCartItemAsync(item);

                return;
            }

            await _cartRepository
                .UpdateCartItemAsync(item);
        }

        public async Task RemoveFromCartAsync(
            string userId,
            int cartItemId)
        {
            var item = await _cartRepository
                .GetCartItemAsync(
                    userId,
                    cartItemId);

            if (item == null)
            {
                return;
            }

            await _cartRepository
                .RemoveCartItemAsync(item);
        }
    }
}