using FoodDeliverySystem.Dtos;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _repository;

        public RestaurantService(
            IRestaurantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RestaurantDto>> GetAllAsync()
        {
            var restaurants = await _repository.GetAllAsync();

            return restaurants
                .Select(MapToDto)
                .ToList();
        }

        public async Task<RestaurantDto?> GetByIdAsync(int id)
        {
            var restaurant = await _repository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return null;
            }

            return MapToDto(restaurant);
        }

        public async Task<RestaurantDto?> GetDetailsAsync(int id)
        {
            var restaurant =
                await _repository.GetDetailsAsync(id);

            if (restaurant == null)
            {
                return null;
            }

            return MapToDto(restaurant);
        }

        public async Task CreateAsync(RestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description,
                Logo = dto.Logo,
                CoverImage = dto.CoverImage,
                IsOpen = dto.IsOpen
            };

            await _repository.AddAsync(restaurant);
        }

        public async Task UpdateAsync(RestaurantDto dto)
        {
            var restaurant =
                await _repository.GetByIdAsync(dto.Id);

            if (restaurant == null)
            {
                throw new InvalidOperationException(
                    "Restaurant not found.");
            }

            restaurant.Name = dto.Name;
            restaurant.Address = dto.Address;
            restaurant.PhoneNumber = dto.PhoneNumber;
            restaurant.Description = dto.Description;
            restaurant.Logo = dto.Logo;
            restaurant.CoverImage = dto.CoverImage;
            restaurant.IsOpen = dto.IsOpen;

            await _repository.UpdateAsync(restaurant);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private static RestaurantDto MapToDto(
            Restaurant restaurant)
        {
            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Description = restaurant.Description,
                Logo = restaurant.Logo,
                CoverImage = restaurant.CoverImage,
                IsOpen = restaurant.IsOpen,

                FoodItems = restaurant.FoodItems?
                    .Select(f => new FoodItemDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        Description = f.Description,
                        ImageUrl = f.ImageUrl,
                        IsAvailable = f.IsAvailable,
                        IsFeatured = f.IsFeatured,
                        RestaurantId = f.RestaurantId,
                        FoodCategoryId = f.FoodCategoryId,
                        RestaurantName =
                            restaurant.Name,
                        FoodCategoryName =
                            f.FoodCategory?.Name ?? string.Empty
                    })
                    .ToList()
                    ?? new List<FoodItemDto>()
            };
        }
    }
}