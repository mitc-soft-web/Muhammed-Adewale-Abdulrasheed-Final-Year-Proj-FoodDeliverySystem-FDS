using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.ViewModels
{
    public class HomeViewModel
    {
        public List<FoodItem> FoodItems { get; set; } = new();

        public List<Restaurant> Restaurants { get; set; } = new();
    }
}
