using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using StudyShare.ViewModels;
using System; // Cần thêm dòng này để dùng Exception trong khối catch
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudyShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var dtoList = await _categoryService.GetAllAsync();
            var viewModels = _mapper.Map<IEnumerable<CategoryViewModel>>(dtoList);
            return View(viewModels);
        }

        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var request = _mapper.Map<CategoryCreateRequest>(viewModel);
                await _categoryService.CreateAsync(request);
                TempData["Success"] = "Thêm danh mục mới thành công!"; 
                return RedirectToAction(nameof(Index));
           }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoryDto = await _categoryService.GetForEditAsync(id);
            if (categoryDto == null) return NotFound();
            
            // Map từ DTO ra ViewModel để hiển thị lên Form
            var viewModel = _mapper.Map<CategoryEditViewModel>(categoryDto);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var request = _mapper.Map<CategoryUpdateRequest>(viewModel);
                await _categoryService.UpdateAsync(request);
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) 
        {
            try 
            {
                var success = await _categoryService.DeleteAsync(id);
                
                if (success) 
                {
                    TempData["Success"] = "Đã xóa danh mục thành công!";
                }
                else 
                {
                    // Bây giờ nếu false thì chắc chắn 100% là do ID bị sai hoặc không tồn tại
                    TempData["Error"] = "Xóa thất bại! Danh mục không tồn tại trên hệ thống.";
                }
            }
            catch (InvalidOperationException ex)
            {
                // "Chụp" đúng cái lỗi bị vướng dữ liệu từ Repository ném lên
                TempData["Error"] = "Không thể xóa! " + ex.Message;
            }
            catch (Exception)
            {
                // Bắt các lỗi Database ngầm khác (nếu có)
                TempData["Error"] = "Có lỗi hệ thống xảy ra khi xóa danh mục.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}