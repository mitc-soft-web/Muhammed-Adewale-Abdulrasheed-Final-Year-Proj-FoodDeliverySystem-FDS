using FoodDeliverySystem.DTOs.Home;

namespace FoodDeliverySystem.Services
{
    public interface IHomeService
    {
        Task<HomeDto> GetHomeDataAsync();
    }
}