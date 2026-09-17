using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IFoodItemRepository
    {
        Task<List<FoodItem>> GetAllAsync();

        Task<FoodItem?> GetByIdAsync(int id);

        Task CreateAsync(
            FoodItem foodItem);

        Task UpdateAsync(
            FoodItem foodItem);

        Task DeleteAsync(
            int id);

        Task<List<FoodCategory>> GetFoodCategoriesAsync();

        Task<List<Restaurant>> GetRestaurantsAsync();
    }
}