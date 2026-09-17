using FoodDeliverySystem.Dtos;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodDeliverySystem.Controllers
{
    public class FoodItemController : Controller
    {
        private readonly IFoodItemService _foodItemService;
        private readonly IWebHostEnvironment _environment;

        public FoodItemController(
            IFoodItemService foodItemService,
            IWebHostEnvironment environment)
        {
            _foodItemService = foodItemService;
            _environment = environment;
        }

        // =========================================================
        // PUBLIC
        // =========================================================

        public async Task<IActionResult> Index()
        {
            var foodItems =
                await _foodItemService.GetAllAsync();

            // Existing view expects FoodItem models.
            // Therefore DTOs are mapped back to the exact
            // model shape expected by the existing view.

            var models = foodItems
                .Select(dto => new FoodItem
                {
                    Id =
                        dto.Id,

                    Name =
                        dto.Name,

                    Price =
                        dto.Price,

                    Description =
                        dto.Description,

                    ImageUrl =
                        dto.ImageUrl,

                    IsAvailable =
                        dto.IsAvailable,

                    IsFeatured =
                        dto.IsFeatured,

                    RestaurantId =
                        dto.RestaurantId,

                    FoodCategoryId =
                        dto.FoodCategoryId,

                    Restaurant =
                        new Restaurant
                        {
                            Id =
                                dto.RestaurantId,

                            Name =
                                dto.RestaurantName
                                ?? string.Empty,

                            IsOpen =
                                dto.RestaurantIsOpen
                        },

                    FoodCategory =
                        new FoodCategory
                        {
                            Id =
                                dto.FoodCategoryId,

                            Name =
                                dto.FoodCategoryName
                                ?? string.Empty
                        }
                })
                .ToList();

            return View(models);
        }

        // =========================================================
        // CREATE
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            FoodItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(
                    model.FoodCategoryId,
                    model.RestaurantId);

                return View(model);
            }

            string? imagePath = null;

            if (model.ImageFile != null)
            {
                imagePath =
                    await SaveFoodImageAsync(
                        model.ImageFile);
            }

            var dto = new FoodItemDto
            {
                Name =
                    model.Name,

                Price =
                    model.Price,

                Description =
                    model.Description,

                ImageUrl =
                    imagePath,

                IsAvailable =
                    model.IsAvailable,

                IsFeatured =
                    model.IsFeatured,

                RestaurantId =
                    model.RestaurantId,

                FoodCategoryId =
                    model.FoodCategoryId
            };

            await _foodItemService.CreateAsync(
                dto);

            return RedirectToAction(
                nameof(Index));
        }

        // =========================================================
        // EDIT
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _foodItemService.GetByIdAsync(
                    id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var model =
                new FoodItemViewModel
                {
                    Id =
                        dto.Id,

                    Name =
                        dto.Name,

                    Price =
                        dto.Price,

                    Description =
                        dto.Description,

                    IsAvailable =
                        dto.IsAvailable,

                    IsFeatured =
                        dto.IsFeatured,

                    RestaurantId =
                        dto.RestaurantId,

                    FoodCategoryId =
                        dto.FoodCategoryId,

                    ImageUrl =
                        dto.ImageUrl
                };

            await LoadDropdownsAsync(
                model.FoodCategoryId,
                model.RestaurantId);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            FoodItemViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(
                    model.FoodCategoryId,
                    model.RestaurantId);

                return View(model);
            }

            // Retrieve existing food item so that an existing
            // image is preserved when no new image is uploaded.

            var existing =
                await _foodItemService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            string? imagePath =
                existing.ImageUrl;

            if (model.ImageFile != null)
            {
                imagePath =
                    await SaveFoodImageAsync(
                        model.ImageFile);
            }

            var dto = new FoodItemDto
            {
                Id =
                    model.Id,

                Name =
                    model.Name,

                Price =
                    model.Price,

                Description =
                    model.Description,

                ImageUrl =
                    imagePath,

                IsAvailable =
                    model.IsAvailable,

                IsFeatured =
                    model.IsFeatured,

                RestaurantId =
                    model.RestaurantId,

                FoodCategoryId =
                    model.FoodCategoryId
            };

            await _foodItemService.UpdateAsync(
                dto);

            return RedirectToAction(
                nameof(Index));
        }

        // =========================================================
        // DELETE
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _foodItemService.GetByIdAsync(
                    id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            // Existing Delete view expects FoodItem.
            // Map DTO back to the existing model shape.

            var model = new FoodItem
            {
                Id =
                    dto.Id,

                Name =
                    dto.Name,

                Price =
                    dto.Price,

                Description =
                    dto.Description,

                ImageUrl =
                    dto.ImageUrl,

                IsAvailable =
                    dto.IsAvailable,

                IsFeatured =
                    dto.IsFeatured,

                RestaurantId =
                    dto.RestaurantId,

                FoodCategoryId =
                    dto.FoodCategoryId,

                Restaurant =
                    new Restaurant
                    {
                        Id =
                            dto.RestaurantId,

                        Name =
                            dto.RestaurantName
                            ?? string.Empty,

                        IsOpen =
                            dto.RestaurantIsOpen
                    },

                FoodCategory =
                    new FoodCategory
                    {
                        Id =
                            dto.FoodCategoryId,

                        Name =
                            dto.FoodCategoryName
                            ?? string.Empty
                    }
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _foodItemService.DeleteAsync(id);

            return RedirectToAction(
                nameof(Index));
        }

        // =========================================================
        // DROPDOWNS
        // =========================================================

        private async Task LoadDropdownsAsync(
            int? selectedCategoryId = null,
            int? selectedRestaurantId = null)
        {
            var categories =
                await _foodItemService
                    .GetFoodCategoriesAsync();

            var restaurants =
                await _foodItemService
                    .GetRestaurantsAsync();

            ViewBag.FoodCategories =
                new SelectList(
                    categories,
                    "Id",
                    "Name",
                    selectedCategoryId);

            ViewBag.Restaurants =
                new SelectList(
                    restaurants,
                    "Id",
                    "Name",
                    selectedRestaurantId);
        }

        // =========================================================
        // FOOD IMAGE UPLOAD
        // =========================================================

        private async Task<string>
            SaveFoodImageAsync(
                IFormFile imageFile)
        {
            string uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "foods");

            Directory.CreateDirectory(
                uploadsFolder);

            string fileName =
                Guid.NewGuid().ToString()
                + Path.GetExtension(
                    imageFile.FileName);

            string filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);

            using (var stream =
                new FileStream(
                    filePath,
                    FileMode.Create))
            {
                await imageFile.CopyToAsync(
                    stream);
            }

            return "/images/foods/" + fileName;
        }
    }
}