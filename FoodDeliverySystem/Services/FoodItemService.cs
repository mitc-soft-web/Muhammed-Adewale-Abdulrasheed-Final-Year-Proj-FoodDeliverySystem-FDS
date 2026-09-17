using FoodDeliverySystem.Dtos;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class FoodItemService : IFoodItemService
    {
        private readonly IFoodItemRepository _repository;

        public FoodItemService(
            IFoodItemRepository repository)
        {
            _repository = repository;
        }

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<List<FoodItemDto>>
            GetAllAsync()
        {
            var foodItems =
                await _repository.GetAllAsync();

            return foodItems
                .Select(MapToDto)
                .ToList();
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<FoodItemDto?>
            GetByIdAsync(int id)
        {
            var foodItem =
                await _repository.GetByIdAsync(id);

            if (foodItem == null)
            {
                return null;
            }

            return MapToDto(foodItem);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task CreateAsync(
            FoodItemDto dto)
        {
            var foodItem = new FoodItem
            {
                Name =
                    dto.Name,

                Price =
                    dto.Price,

                Description =
                    dto.Description,

                ImageUrl =
                    dto.ImageUrl,

                IsAvailable =
                    dto.IsAvailable,

                IsFeatured =
                    dto.IsFeatured,

                RestaurantId =
                    dto.RestaurantId,

                FoodCategoryId =
                    dto.FoodCategoryId
            };

            await _repository.CreateAsync(
                foodItem);
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(
            FoodItemDto dto)
        {
            var foodItem = new FoodItem
            {
                Id =
                    dto.Id,

                Name =
                    dto.Name,

                Price =
                    dto.Price,

                Description =
                    dto.Description,

                ImageUrl =
                    dto.ImageUrl,

                IsAvailable =
                    dto.IsAvailable,

                IsFeatured =
                    dto.IsFeatured,

                RestaurantId =
                    dto.RestaurantId,

                FoodCategoryId =
                    dto.FoodCategoryId
            };

            await _repository.UpdateAsync(
                foodItem);
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task DeleteAsync(
            int id)
        {
            await _repository.DeleteAsync(id);
        }

        // =========================================================
        // FOOD CATEGORY DROPDOWN
        // =========================================================

        public async Task<List<FoodCategoryOptionDto>>
            GetFoodCategoriesAsync()
        {
            var categories =
                await _repository
                    .GetFoodCategoriesAsync();

            return categories
                .Select(category =>
                    new FoodCategoryOptionDto
                    {
                        Id =
                            category.Id,

                        Name =
                            category.Name
                    })
                .ToList();
        }

        // =========================================================
        // RESTAURANT DROPDOWN
        // =========================================================

        public async Task<List<RestaurantOptionDto>>
            GetRestaurantsAsync()
        {
            var restaurants =
                await _repository
                    .GetRestaurantsAsync();

            return restaurants
                .Select(restaurant =>
                    new RestaurantOptionDto
                    {
                        Id =
                            restaurant.Id,

                        Name =
                            restaurant.Name,

                        IsOpen =
                            restaurant.IsOpen
                    })
                .ToList();
        }

        // =========================================================
        // MODEL -> DTO
        // =========================================================

        private static FoodItemDto
            MapToDto(
                FoodItem foodItem)
        {
            return new FoodItemDto
            {
                Id =
                    foodItem.Id,

                Name =
                    foodItem.Name,

                Price =
                    foodItem.Price,

                Description =
                    foodItem.Description,

                ImageUrl =
                    foodItem.ImageUrl,

                IsAvailable =
                    foodItem.IsAvailable,

                IsFeatured =
                    foodItem.IsFeatured,

                RestaurantId =
                    foodItem.RestaurantId,

                RestaurantName =
                    foodItem.Restaurant?.Name,

                RestaurantIsOpen =
                    foodItem.Restaurant?.IsOpen
                    ?? false,

                FoodCategoryId =
                    foodItem.FoodCategoryId,

                FoodCategoryName =
                    foodItem.FoodCategory?.Name
            };
        }
    }
}