using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class FoodCategoryRepository : IFoodCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FoodCategory>> GetAllAsync()
        {
            return await _context.FoodCategories
                .Include(c => c.FoodItems)
                .ToListAsync();
        }

        public async Task<FoodCategory?> GetByIdAsync(int id)
        {
            return await _context.FoodCategories
                .Include(c => c.FoodItems)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(FoodCategory category)
        {
            _context.FoodCategories.Add(category);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FoodCategory category)
        {
            _context.FoodCategories.Update(category);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.FoodCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return;
            }

            _context.FoodCategories.Remove(category);

            await _context.SaveChangesAsync();
        }
    }
}