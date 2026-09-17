using FoodDeliverySystem.Dtos.FoodCategory;

namespace FoodDeliverySystem.Services
{
    public interface IFoodCategoryService
    {
        Task<List<FoodCategoryDto>> GetAllAsync();

        Task<FoodCategoryDto?> GetByIdAsync(int id);

        Task CreateAsync(FoodCategoryDto dto);

        Task UpdateAsync(FoodCategoryDto dto);

        Task DeleteAsync(int id);
    }
}