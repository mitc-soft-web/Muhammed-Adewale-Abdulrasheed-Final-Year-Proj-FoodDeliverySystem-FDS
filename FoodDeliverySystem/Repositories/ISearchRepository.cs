using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Repositories
{
    public interface ISearchRepository
    {
        Task<List<FoodItem>> SearchFoodsAsync(string keyword);

        Task<List<FoodItem>> GetSuggestionFoodsAsync(string keyword);

        Task<List<Restaurant>> GetSuggestionRestaurantsAsync(string keyword);

        Task<List<FoodCategory>> GetSuggestionCategoriesAsync(string keyword);

        Task<List<SearchHistory>> GetRecentSearchesAsync(string userId);

        Task<List<(string Text, int Count)>> GetTrendingSearchesAsync();

        Task<SearchHistory?> GetExistingSearchAsync(
            string userId,
            string searchText);

        Task AddSearchAsync(SearchHistory searchHistory);

        Task<List<SearchHistory>> GetUserSearchHistoryAsync(
            string userId);

        Task DeleteSearchAsync(SearchHistory searchHistory);

        Task<List<SearchHistory>> GetAllUserSearchHistoryAsync(
            string userId);

        Task SaveChangesAsync();
    }
}