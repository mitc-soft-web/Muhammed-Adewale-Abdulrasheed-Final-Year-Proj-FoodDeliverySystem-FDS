using FoodDeliverySystem.DTOs.Search;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchRepository _repository;

        public SearchService(
            ISearchRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FoodItem>> SearchFoodsAsync(
            string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<FoodItem>();
            }

            return await _repository.SearchFoodsAsync(keyword);
        }

        public async Task<List<SearchSuggestionDto>> GetSuggestionsAsync(
            string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<SearchSuggestionDto>();
            }

            var results = new List<SearchSuggestionDto>();

            var foods =
                await _repository.GetSuggestionFoodsAsync(keyword);

            foreach (var food in foods)
            {
                results.Add(new SearchSuggestionDto
                {
                    Type = "Food",
                    Text = food.Name,
                    Subtitle =
                        $"{food.Restaurant?.Name} • {food.FoodCategory?.Name}",
                    RestaurantId = food.RestaurantId
                });
            }

            var restaurants =
                await _repository
                    .GetSuggestionRestaurantsAsync(keyword);

            foreach (var restaurant in restaurants)
            {
                results.Add(new SearchSuggestionDto
                {
                    Type = "Restaurant",
                    Text = restaurant.Name,
                    Subtitle = "Restaurant",
                    RestaurantId = restaurant.Id
                });
            }

            var categories =
                await _repository
                    .GetSuggestionCategoriesAsync(keyword);

            foreach (var category in categories)
            {
                results.Add(new SearchSuggestionDto
                {
                    Type = "Category",
                    Text = category.Name,
                    Subtitle = "Category",
                    RestaurantId = 0
                });
            }

            return results;
        }

        public async Task<List<SearchHistoryDto>> GetRecentSearchesAsync(
            string userId)
        {
            var searches =
                await _repository.GetRecentSearchesAsync(userId);

            return searches.Select(x => new SearchHistoryDto
            {
                Id = x.Id,
                Text = x.SearchText
            }).ToList();
        }

        public async Task<List<TrendingSearchDto>> GetTrendingSearchesAsync()
        {
            var searches =
                await _repository.GetTrendingSearchesAsync();

            return searches
                .Select(x => new TrendingSearchDto
                {
                    Text = x.Text,
                    Count = x.Count
                })
                .ToList();
        }

        public async Task SaveSearchAsync(
            string userId,
            string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            text = text.Trim();

            var existing =
                await _repository.GetExistingSearchAsync(
                    userId,
                    text);

            if (existing != null)
            {
                existing.CreatedAt = DateTime.Now;
            }
            else
            {
                await _repository.AddSearchAsync(
                    new SearchHistory
                    {
                        UserId = userId,
                        SearchText = text,
                        CreatedAt = DateTime.Now
                    });
            }

            var histories =
                await _repository
                    .GetUserSearchHistoryAsync(userId);

            if (histories.Count > 10)
            {
                foreach (var oldHistory in histories.Skip(10))
                {
                    await _repository.DeleteSearchAsync(
                        oldHistory);
                }
            }

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteSearchAsync(
            int id,
            string userId)
        {
            var histories =
                await _repository
                    .GetAllUserSearchHistoryAsync(userId);

            var history =
                histories.FirstOrDefault(x => x.Id == id);

            if (history == null)
            {
                return;
            }

            await _repository.DeleteSearchAsync(history);

            await _repository.SaveChangesAsync();
        }

        public async Task ClearSearchHistoryAsync(
            string userId)
        {
            var histories =
                await _repository
                    .GetAllUserSearchHistoryAsync(userId);

            foreach (var history in histories)
            {
                await _repository.DeleteSearchAsync(history);
            }

            await _repository.SaveChangesAsync();
        }
    }
}