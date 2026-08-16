using FoodDeliveryApp.Application.Interfaces;
using FoodDeliveryApp.Application.IServices;
using FoodDeliveryApp.Application.ViewModels.MenuItem;
using FoodDeliveryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodDeliveryApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICloudinaryImageUploadService _uploadService;

        public MenuItemController(IMenuItemRepository menuItemRepository ,
                                  ICategoryRepository categoryRepository ,
                                  ICloudinaryImageUploadService uploadService)
        {
            _menuItemRepository = menuItemRepository;
            _categoryRepository = categoryRepository;
            _uploadService = uploadService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();
            var VMs = menuItems.Select(menuItem => new MenuItemResponseVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                ImageUrl = menuItem.ImageUrl,
                Price = menuItem.Price,
                SpecialPrice = menuItem.SpecialPrice,
                CategoryName = menuItem.Category.Name
            }).ToList();

            return View(VMs);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await LoadCategoriesData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(MenuItemAddVM addVM)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesData();
                return View(addVM);
            }

            string imageUrl;
            string publicId;
            try
            {
                var uploadResult = await _uploadService.UploadImageAsync(addVM.File);
                imageUrl = uploadResult.SecureUrl.ToString();
                publicId = uploadResult.PublicId;
            } catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Image upload failed");
                await LoadCategoriesData();
                return View(addVM);
            }

            var menuItem = new MenuItem
            {
                Name = addVM.Name,
                Description = addVM.Description,
                ImageUrl = imageUrl,
                PublicId = publicId,
                Price = addVM.Price,
                SpecialPrice = addVM.SpecialPrice,
                CategoryId = addVM.CategoryId,
            };

            await _menuItemRepository.AddAsync(menuItem);
            TempData["success"] = "Menu created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if(menuItem == null)
            {
                return NotFound();
            }

            await LoadCategoriesData();

            var vm = new MenuItemEditVM
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                SpecialPrice = menuItem.SpecialPrice,
                ExistingImageUrl = menuItem.ImageUrl,
                CategoryId = menuItem.CategoryId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MenuItemEditVM editVM)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesData();
                return View(editVM);
            }

            var menuItemFromDb = await _menuItemRepository.GetAsync(editVM.Id);
            if(menuItemFromDb == null)
            {
                return NotFound();
            }

            menuItemFromDb.Name = editVM.Name;
            menuItemFromDb.Description = editVM.Description;
            menuItemFromDb.Price = editVM.Price;
            menuItemFromDb.SpecialPrice = editVM.SpecialPrice;
            menuItemFromDb.CategoryId = editVM.CategoryId;

            if(editVM.File != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(menuItemFromDb.PublicId))
                    {
                        await _uploadService.DeleteImageAsync(menuItemFromDb.PublicId);
                    }

                    var uploadResult = await _uploadService.UploadImageAsync(editVM.File);
                    menuItemFromDb.ImageUrl = uploadResult.SecureUrl.ToString();
                    menuItemFromDb.PublicId = uploadResult.PublicId;
                } catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Image upload failed");
                    await LoadCategoriesData();
                    return View(editVM);
                }
            }

            await _menuItemRepository.UpdateAsync();
            TempData["success"] = "Menu updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var menuItem = await _menuItemRepository.GetAsync(id);
            if(menuItem == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(menuItem.PublicId))
            {
                await _uploadService.DeleteImageAsync(menuItem.PublicId);
            }

            await _menuItemRepository.DeleteAsync(id);
            TempData["success"] = "Menu deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesData()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.categories = new SelectList(categories, "Id", "Name");
        }
    }
}
