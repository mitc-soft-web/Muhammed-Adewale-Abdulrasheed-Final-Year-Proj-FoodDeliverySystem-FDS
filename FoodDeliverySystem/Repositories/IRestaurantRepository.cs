using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAllAsync();

        Task<Restaurant?> GetByIdAsync(int id);

        Task<Restaurant?> GetDetailsAsync(int id);

        Task AddAsync(Restaurant restaurant);

        Task UpdateAsync(Restaurant restaurant);

        Task DeleteAsync(int id);
    }
}