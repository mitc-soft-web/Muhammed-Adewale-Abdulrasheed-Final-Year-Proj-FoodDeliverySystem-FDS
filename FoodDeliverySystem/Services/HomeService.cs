using FoodDeliverySystem.DTOs.Home;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _repository;

        public HomeService(
            IHomeRepository repository)
        {
            _repository = repository;
        }

        public async Task<HomeDto>
            GetHomeDataAsync()
        {
            var foodItems =
                await _repository.GetFeaturedFoodItemsAsync();

            var restaurants =
                await _repository.GetRestaurantsAsync();

            return new HomeDto
            {
                FoodItems = foodItems
                    .Select(MapFoodItemToDto)
                    .ToList(),

                Restaurants = restaurants
                    .Select(MapRestaurantToDto)
                    .ToList()
            };
        }

        private static HomeFoodItemDto
            MapFoodItemToDto(FoodItem foodItem)
        {
            return new HomeFoodItemDto
            {
                Id = foodItem.Id,

                Name = foodItem.Name,

                Price = foodItem.Price,

                Description = foodItem.Description,

                ImageUrl = foodItem.ImageUrl,

                IsAvailable = foodItem.IsAvailable,

                IsFeatured = foodItem.IsFeatured,

                RestaurantId = foodItem.RestaurantId,

                RestaurantName =
                    foodItem.Restaurant?.Name,

                RestaurantIsOpen =
                    foodItem.Restaurant?.IsOpen ?? false,

                FoodCategoryId =
                    foodItem.FoodCategoryId,

                FoodCategoryName =
                    foodItem.FoodCategory?.Name
            };
        }

        private static HomeRestaurantDto
            MapRestaurantToDto(Restaurant restaurant)
        {
            return new HomeRestaurantDto
            {
                Id = restaurant.Id,

                Name = restaurant.Name,

                Address = restaurant.Address,

                PhoneNumber = restaurant.PhoneNumber,

                Description = restaurant.Description,

                Logo = restaurant.Logo,

                CoverImage = restaurant.CoverImage,

                IsOpen = restaurant.IsOpen,

                CreatedAt = restaurant.CreatedAt,

                OwnerId = restaurant.OwnerId
            };
        }
    }
}