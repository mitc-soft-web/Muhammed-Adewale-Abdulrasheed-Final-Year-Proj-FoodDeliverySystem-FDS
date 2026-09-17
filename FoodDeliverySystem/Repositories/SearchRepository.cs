using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ApplicationDbContext _context;

        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FoodItem>> SearchFoodsAsync(
            string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _context.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .Where(f =>
                    f.Name.ToLower().Contains(keyword) ||
                    (f.Description != null &&
                     f.Description.ToLower().Contains(keyword)) ||
                    (f.FoodCategory != null &&
                     f.FoodCategory.Name.ToLower().Contains(keyword)) ||
                    (f.Restaurant != null &&
                     f.Restaurant.Name.ToLower().Contains(keyword)))
                .OrderBy(f =>
                    !f.Name.ToLower().StartsWith(keyword))
                .ThenBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<List<FoodItem>> GetSuggestionFoodsAsync(
            string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _context.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .Where(f =>
                    f.Name.ToLower().Contains(keyword) ||
                    (f.Description != null &&
                     f.Description.ToLower().Contains(keyword)))
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Restaurant>> GetSuggestionRestaurantsAsync(
            string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _context.Restaurants
                .Where(r =>
                    r.Name.ToLower().Contains(keyword))
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<FoodCategory>> GetSuggestionCategoriesAsync(
            string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _context.FoodCategories
                .Where(c =>
                    c.Name.ToLower().Contains(keyword))
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<SearchHistory>> GetRecentSearchesAsync(
            string userId)
        {
            return await _context.SearchHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<(string Text, int Count)>> GetTrendingSearchesAsync()
        {
            return await _context.SearchHistories
                .GroupBy(x => x.SearchText)
                .Select(g => new
                {
                    Text = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Text)
                .Take(8)
                .Select(x => new ValueTuple<string, int>(
                    x.Text,
                    x.Count))
                .ToListAsync();
        }

        public async Task<SearchHistory?> GetExistingSearchAsync(
            string userId,
            string searchText)
        {
            searchText = searchText.Trim().ToLower();

            return await _context.SearchHistories
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SearchText.ToLower() == searchText);
        }

        public async Task AddSearchAsync(
            SearchHistory searchHistory)
        {
            await _context.SearchHistories.AddAsync(searchHistory);
        }

        public async Task<List<SearchHistory>> GetUserSearchHistoryAsync(
            string userId)
        {
            return await _context.SearchHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteSearchAsync(
            SearchHistory searchHistory)
        {
            _context.SearchHistories.Remove(searchHistory);

            await Task.CompletedTask;
        }

        public async Task<List<SearchHistory>> GetAllUserSearchHistoryAsync(
            string userId)
        {
            return await _context.SearchHistories
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}