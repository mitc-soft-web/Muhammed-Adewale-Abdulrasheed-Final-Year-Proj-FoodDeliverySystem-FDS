using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IHomeRepository
    {
        Task<List<FoodItem>> GetFeaturedFoodItemsAsync();

        Task<List<Restaurant>> GetRestaurantsAsync();
    }
}