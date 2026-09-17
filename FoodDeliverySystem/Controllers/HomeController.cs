using System.Diagnostics;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(
            IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            // =========================================================
            // AUTHENTICATED USER REDIRECTS
            // =========================================================

            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Admin");
                }

                if (User.IsInRole("Rider"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Rider");
                }

                if (User.IsInRole("RestaurantOwner"))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Admin");
                }
            }

            // =========================================================
            // CUSTOMER / GUEST HOME PAGE
            // =========================================================

            var homeData =
                await _homeService.GetHomeDataAsync();

            var model =
                new HomeViewModel
                {
                    FoodItems =
                        homeData.FoodItems
                            .Select(food => new FoodItem
                            {
                                Id = food.Id,

                                Name = food.Name,

                                Price = food.Price,

                                Description =
                                    food.Description,

                                ImageUrl =
                                    food.ImageUrl,

                                IsAvailable =
                                    food.IsAvailable,

                                IsFeatured =
                                    food.IsFeatured,

                                RestaurantId =
                                    food.RestaurantId,

                                FoodCategoryId =
                                    food.FoodCategoryId,

                                Restaurant =
                                    new Restaurant
                                    {
                                        Id =
                                            food.RestaurantId,

                                        Name =
                                            food.RestaurantName
                                            ?? string.Empty,

                                        IsOpen =
                                            food.RestaurantIsOpen
                                    },

                                FoodCategory =
                                    new FoodCategory
                                    {
                                        Id =
                                            food.FoodCategoryId,

                                        Name =
                                            food.FoodCategoryName
                                            ?? string.Empty
                                    }
                            })
                            .ToList(),

                    Restaurants =
                        homeData.Restaurants
                            .Select(restaurant =>
                                new Restaurant
                                {
                                    Id =
                                        restaurant.Id,

                                    Name =
                                        restaurant.Name,

                                    Address =
                                        restaurant.Address,

                                    PhoneNumber =
                                        restaurant.PhoneNumber,

                                    Description =
                                        restaurant.Description,

                                    Logo =
                                        restaurant.Logo,

                                    CoverImage =
                                        restaurant.CoverImage,

                                    IsOpen =
                                        restaurant.IsOpen,

                                    CreatedAt =
                                        restaurant.CreatedAt,

                                    OwnerId =
                                        restaurant.OwnerId
                                })
                            .ToList()
                };

            return View(model);
        }

        // =========================================================
        // ERROR
        // =========================================================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}