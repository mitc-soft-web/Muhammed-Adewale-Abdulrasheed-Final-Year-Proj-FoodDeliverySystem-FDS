using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodItemRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL FOOD ITEMS
        // =========================================================

        public async Task<List<FoodItem>>
            GetAllAsync()
        {
            return await _context.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        // =========================================================
        // GET FOOD ITEM BY ID
        // =========================================================

        public async Task<FoodItem?>
            GetByIdAsync(int id)
        {
            return await _context.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .FirstOrDefaultAsync(
                    f => f.Id == id);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task CreateAsync(
            FoodItem foodItem)
        {
            _context.FoodItems.Add(foodItem);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(
            FoodItem foodItem)
        {
            var existingFoodItem =
                await _context.FoodItems
                    .FirstOrDefaultAsync(
                        f => f.Id == foodItem.Id);

            if (existingFoodItem == null)
            {
                return;
            }

            existingFoodItem.Name =
                foodItem.Name;

            existingFoodItem.Price =
                foodItem.Price;

            existingFoodItem.Description =
                foodItem.Description;

            existingFoodItem.ImageUrl =
                foodItem.ImageUrl;

            existingFoodItem.IsAvailable =
                foodItem.IsAvailable;

            existingFoodItem.IsFeatured =
                foodItem.IsFeatured;

            existingFoodItem.RestaurantId =
                foodItem.RestaurantId;

            existingFoodItem.FoodCategoryId =
                foodItem.FoodCategoryId;

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task DeleteAsync(
            int id)
        {
            var foodItem =
                await _context.FoodItems
                    .FirstOrDefaultAsync(
                        f => f.Id == id);

            if (foodItem == null)
            {
                return;
            }

            _context.FoodItems.Remove(foodItem);

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // FOOD CATEGORIES
        // =========================================================

        public async Task<List<FoodCategory>>
            GetFoodCategoriesAsync()
        {
            return await _context.FoodCategories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // =========================================================
        // RESTAURANTS
        // =========================================================

        public async Task<List<Restaurant>>
            GetRestaurantsAsync()
        {
            return await _context.Restaurants
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}