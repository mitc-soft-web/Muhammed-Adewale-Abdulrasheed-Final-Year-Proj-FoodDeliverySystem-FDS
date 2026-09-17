using FoodDeliverySystem.Dtos.FoodCategory;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class FoodCategoryService : IFoodCategoryService
    {
        private readonly IFoodCategoryRepository _repository;

        public FoodCategoryService(
            IFoodCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FoodCategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories
                .Select(MapToDto)
                .ToList();
        }

        public async Task<FoodCategoryDto?> GetByIdAsync(int id)
        {
            var category =
                await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return MapToDto(category);
        }

        public async Task CreateAsync(FoodCategoryDto dto)
        {
            var category = new FoodCategory
            {
                Name = dto.Name
            };

            await _repository.AddAsync(category);
        }

        public async Task UpdateAsync(FoodCategoryDto dto)
        {
            var category =
                await _repository.GetByIdAsync(dto.Id);

            if (category == null)
            {
                return;
            }

            category.Name = dto.Name;

            await _repository.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private static FoodCategoryDto MapToDto(
            FoodCategory category)
        {
            return new FoodCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                FoodItemCount =
                    category.FoodItems?.Count ?? 0
            };
        }
    }
}