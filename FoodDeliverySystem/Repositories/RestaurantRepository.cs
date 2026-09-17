using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Restaurant?> GetByIdAsync(int id)
        {
            return await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Restaurant?> GetDetailsAsync(int id)
        {
            return await _context.Restaurants
                .Include(r => r.FoodItems)
                    .ThenInclude(f => f.FoodCategory)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Restaurant restaurant)
        {
            _context.Restaurants.Add(restaurant);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return;
            }

            _context.Restaurants.Remove(restaurant);

            await _context.SaveChangesAsync();
        }
    }
}