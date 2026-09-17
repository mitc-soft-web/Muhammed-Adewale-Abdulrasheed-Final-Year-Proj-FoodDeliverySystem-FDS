using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface IFoodCategoryRepository
    {
        Task<List<FoodCategory>> GetAllAsync();

        Task<FoodCategory?> GetByIdAsync(int id);

        Task AddAsync(FoodCategory category);

        Task UpdateAsync(FoodCategory category);

        Task DeleteAsync(int id);
    }
}