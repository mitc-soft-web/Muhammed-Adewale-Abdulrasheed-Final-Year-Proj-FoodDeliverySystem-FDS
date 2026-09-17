using FoodDeliverySystem.Dtos;

namespace FoodDeliverySystem.Services
{
    public interface IRestaurantService
    {
        Task<List<RestaurantDto>> GetAllAsync();

        Task<RestaurantDto?> GetByIdAsync(int id);

        Task<RestaurantDto?> GetDetailsAsync(int id);

        Task CreateAsync(RestaurantDto dto);

        Task UpdateAsync(RestaurantDto dto);

        Task DeleteAsync(int id);
    }
}