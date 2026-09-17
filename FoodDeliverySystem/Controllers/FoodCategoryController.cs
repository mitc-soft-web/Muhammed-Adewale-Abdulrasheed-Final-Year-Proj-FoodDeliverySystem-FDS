using FoodDeliverySystem.Dtos.FoodCategory;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class FoodCategoryController : Controller
    {
        private readonly IFoodCategoryService _categoryService;

        public FoodCategoryController(
            IFoodCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ==========================================
        // PUBLIC
        // ==========================================

        public async Task<IActionResult> Index()
        {
            var categories =
                await _categoryService.GetAllAsync();

            // Existing view expects FoodCategory models.
            // Keep the view untouched.
            var models = categories
                .Select(dto => new FoodCategory
                {
                    Id = dto.Id,
                    Name = dto.Name
                })
                .ToList();

            ViewBag.TotalFoods =
                categories.Sum(c => c.FoodItemCount);

            return View(models);
        }


        // ==========================================
        // ADMIN ONLY
        // ==========================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            FoodCategory category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var dto = new FoodCategoryDto
            {
                Name = category.Name
            };

            await _categoryService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EDIT
        // ==========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _categoryService.GetByIdAsync(id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var category = new FoodCategory
            {
                Id = dto.Id,
                Name = dto.Name
            };

            return View(category);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            FoodCategory category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var existing =
                await _categoryService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            var dto = new FoodCategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            await _categoryService.UpdateAsync(dto);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // DELETE
        // ==========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dto =
                await _categoryService.GetByIdAsync(id.Value);

            if (dto == null)
            {
                return NotFound();
            }

            var category = new FoodCategory
            {
                Id = dto.Id,
                Name = dto.Name
            };

            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _categoryService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}