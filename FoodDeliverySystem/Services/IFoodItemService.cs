using FoodDeliverySystem.Dtos;

namespace FoodDeliverySystem.Services
{
    public interface IFoodItemService
    {
        Task<List<FoodItemDto>>
            GetAllAsync();

        Task<FoodItemDto?>
            GetByIdAsync(int id);

        Task CreateAsync(
            FoodItemDto dto);

        Task UpdateAsync(
            FoodItemDto dto);

        Task DeleteAsync(
            int id);

        Task<List<FoodCategoryOptionDto>>
            GetFoodCategoriesAsync();

        Task<List<RestaurantOptionDto>>
            GetRestaurantsAsync();
    }
}