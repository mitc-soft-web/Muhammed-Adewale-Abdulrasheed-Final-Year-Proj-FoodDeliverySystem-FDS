using FoodDeliverySystem.DTOs.Search;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Services
{
    public interface ISearchService
    {
        Task<List<FoodItem>> SearchFoodsAsync(
            string keyword);

        Task<List<SearchSuggestionDto>> GetSuggestionsAsync(
            string keyword);

        Task<List<SearchHistoryDto>> GetRecentSearchesAsync(
            string userId);

        Task<List<TrendingSearchDto>> GetTrendingSearchesAsync();

        Task SaveSearchAsync(
            string userId,
            string text);

        Task DeleteSearchAsync(
            int id,
            string userId);

        Task ClearSearchHistoryAsync(
            string userId);
    }
}