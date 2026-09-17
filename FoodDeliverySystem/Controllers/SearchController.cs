using FoodDeliverySystem.DTOs.Search;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(
            ISearchService searchService,
            UserManager<ApplicationUser> userManager)
        {
            _searchService = searchService;
            _userManager = userManager;
        }

        // =========================================================
        // SEARCH RESULTS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View(new List<FoodItem>());
            }

            var foods = await _searchService.SearchFoodsAsync(keyword);

            ViewBag.Keyword = keyword;

            return View(foods);
        }


        // =========================================================
        // SEARCH SUGGESTIONS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Suggestions(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Json(new List<SearchSuggestionDto>());
            }

            var suggestions =
                await _searchService.GetSuggestionsAsync(keyword);

            return Json(suggestions);
        }


        // =========================================================
        // RECENT SEARCHES
        // =========================================================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Recent()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new List<SearchHistoryDto>());
            }

            var searches =
                await _searchService.GetRecentSearchesAsync(user.Id);

            return Json(searches);
        }


        // =========================================================
        // TRENDING SEARCHES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Trending()
        {
            var trending =
                await _searchService.GetTrendingSearchesAsync();

            return Json(trending);
        }


        // =========================================================
        // SAVE SEARCH
        // =========================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string text)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest();
            }

            await _searchService.SaveSearchAsync(
                user.Id,
                text);

            return Ok();
        }


        // =========================================================
        // DELETE SEARCH HISTORY ITEM
        // =========================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            await _searchService.DeleteSearchAsync(
                id,
                user.Id);

            return Ok();
        }


        // =========================================================
        // CLEAR SEARCH HISTORY
        // =========================================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            await _searchService.ClearSearchHistoryAsync(
                user.Id);

            return Ok();
        }
    }
}