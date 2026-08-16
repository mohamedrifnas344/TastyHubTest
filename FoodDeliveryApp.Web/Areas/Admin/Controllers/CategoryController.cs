using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Application.ViewModels.Category;
using FoodDeliveryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICloudinaryImageUploadService _uploadService;

        public CategoryController(ICategoryRepository categoryRepository ,
                                  ICloudinaryImageUploadService uploadService)
        {
            _categoryRepository = categoryRepository;
            _uploadService = uploadService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            //Map to VM
            var categoryVMs = categories.Select(x => new CategoryResponseVM
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
            }).ToList();
            return View(categoryVMs);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddVM addVM)
        {
            if (!ModelState.IsValid)
            {
                return View(addVM);
            }

            string imageUrl;
            string publicId;
            try
            {
                var uploadResult = await _uploadService.UploadImageAsync(addVM.File);
                imageUrl = uploadResult.SecureUrl.ToString();
                publicId = uploadResult.PublicId;
            }catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Image upload failed");
                return View(addVM);
            }

            //Map to Domain
            var category = new Category
            {
                Name = addVM.Name,
                ImageUrl = imageUrl,
                PublicId = publicId
            };

            await _categoryRepository.AddAsync(category);
            TempData["success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryRepository.GetAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            var vm = new CategoryEditVM
            {
                Id = category.Id,
                Name = category.Name,
                ExistingImageUrl = category.ImageUrl,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditVM editVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editVM);
            }

            var categoryFromDb = await _categoryRepository.GetAsync(editVM.Id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }

            categoryFromDb.Name = editVM.Name;
            if(editVM.File != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(categoryFromDb.PublicId))
                    {
                        await _uploadService.DeleteImageAsync(categoryFromDb.PublicId);
                    }

                    var uploadResult = await _uploadService.UploadImageAsync(editVM.File);
                    categoryFromDb.ImageUrl = uploadResult.SecureUrl.ToString();
                    categoryFromDb.PublicId = uploadResult.PublicId;
                } catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Image upload failed");
                    return View(editVM);
                }
            }

            await _categoryRepository.UpdateAsync(categoryFromDb);
            TempData["success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryRepository.GetAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.PublicId))
            {
                await _uploadService.DeleteImageAsync(category.PublicId);
            }

            await _categoryRepository.DeleteAsync(id);
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
