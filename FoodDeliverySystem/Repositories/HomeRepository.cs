using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;

        public HomeRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FoodItem>>
            GetFeaturedFoodItemsAsync()
        {
            return await _context.FoodItems
                .Where(f => f.IsFeatured)
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .Take(8)
                .ToListAsync();
        }

        public async Task<List<Restaurant>>
            GetRestaurantsAsync()
        {
            return await _context.Restaurants
                .ToListAsync();
        }
    }
}