using FoodDeliverySystem.Dtos;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IWebHostEnvironment _environment;

        public RestaurantController(
            IRestaurantService restaurantService,
            IWebHostEnvironment environment)
        {
            _restaurantService = restaurantService;
            _environment = environment;
        }

        // ==============================
        // PUBLIC
        // ==============================

        public async Task<IActionResult> Index()
        {
            var restaurants =
                await _restaurantService.GetAllAsync();

            // Existing view expects Restaurant models.
            var models = restaurants
                .Select(dto => new Restaurant
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    Description = dto.Description,
                    Logo = dto.Logo,
                    CoverImage = dto.CoverImage,
                    IsOpen = dto.IsOpen
                })
                .ToList();

            return View(models);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _restaurantService.GetDetailsAsync(id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var restaurant = new Restaurant
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description,
                Logo = dto.Logo,
                CoverImage = dto.CoverImage,
                IsOpen = dto.IsOpen,

                FoodItems = dto.FoodItems
                    .Select(f => new FoodItem
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        Description = f.Description,
                        ImageUrl = f.ImageUrl,
                        IsAvailable = f.IsAvailable,
                        IsFeatured = f.IsFeatured,
                        RestaurantId = f.RestaurantId,
                        FoodCategoryId = f.FoodCategoryId,

                        FoodCategory = new FoodCategory
                        {
                            Id = f.FoodCategoryId,
                            Name = f.FoodCategoryName
                        }
                    })
                    .ToList()
            };

            return View(restaurant);
        }


        // ==============================
        // CREATE
        // ==============================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            FoodDeliverySystem.ViewModels.RestaurantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? logoPath = null;
            string? coverImagePath = null;

            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "restaurants");

            Directory.CreateDirectory(uploadsFolder);


            // LOGO
            if (model.LogoFile != null)
            {
                logoPath =
                    await SaveRestaurantImageAsync(
                        model.LogoFile,
                        uploadsFolder);
            }


            // COVER IMAGE
            if (model.CoverImageFile != null)
            {
                coverImagePath =
                    await SaveRestaurantImageAsync(
                        model.CoverImageFile,
                        uploadsFolder);
            }


            var dto = new RestaurantDto
            {
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                IsOpen = model.IsOpen,
                Logo = logoPath,
                CoverImage = coverImagePath
            };

            await _restaurantService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }


        // ==============================
        // EDIT
        // ==============================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _restaurantService.GetByIdAsync(id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var model =
                new FoodDeliverySystem.ViewModels.RestaurantViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    Description = dto.Description,
                    IsOpen = dto.IsOpen,
                    Logo = dto.Logo,
                    CoverImage = dto.CoverImage
                };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            FoodDeliverySystem.ViewModels.RestaurantViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing =
                await _restaurantService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            string? logoPath = existing.Logo;
            string? coverImagePath = existing.CoverImage;

            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "restaurants");

            Directory.CreateDirectory(uploadsFolder);


            // NEW LOGO
            if (model.LogoFile != null)
            {
                logoPath =
                    await SaveRestaurantImageAsync(
                        model.LogoFile,
                        uploadsFolder);
            }


            // NEW COVER
            if (model.CoverImageFile != null)
            {
                coverImagePath =
                    await SaveRestaurantImageAsync(
                        model.CoverImageFile,
                        uploadsFolder);
            }


            var dto = new RestaurantDto
            {
                Id = model.Id,
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                IsOpen = model.IsOpen,
                Logo = logoPath,
                CoverImage = coverImagePath
            };

            await _restaurantService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }


        // ==============================
        // DELETE
        // ==============================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _restaurantService.GetByIdAsync(id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var restaurant = new Restaurant
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description,
                Logo = dto.Logo,
                CoverImage = dto.CoverImage,
                IsOpen = dto.IsOpen
            };

            return View(restaurant);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _restaurantService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }


        // ==============================
        // IMAGE HELPER
        // ==============================

        private async Task<string> SaveRestaurantImageAsync(
            IFormFile imageFile,
            string uploadsFolder)
        {
            string fileName =
                Guid.NewGuid().ToString() +
                Path.GetExtension(imageFile.FileName);

            string filePath =
                Path.Combine(uploadsFolder, fileName);

            using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/restaurants/" + fileName;
        }
    }
}