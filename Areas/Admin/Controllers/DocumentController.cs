using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyShare.Services.Interfaces;
using StudyShare.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudyShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;

        public DocumentController(IDocumentService documentService, IMapper mapper)
        {
            _documentService = documentService;
            _mapper = mapper;
        }

public async Task<IActionResult> Index(string searchString, string status = "all")
{
    // Đảm bảo status không bị null để tránh lỗi .ToLower()
    status = string.IsNullOrEmpty(status) ? "all" : status;

    ViewData["CurrentFilter"] = searchString;
    ViewBag.CurrentStatus = status;

    // Lấy dữ liệu từ Service (Đảm bảo Include User và Category để lấy Tên)
    var documents = await _documentService.GetAllAsync(); 
    
    // Ánh xạ sang ViewModel (Giả sử bạn dùng AutoMapper)
    var viewModels = _mapper.Map<IEnumerable<DocumentViewModel>>(documents);

    // 1. Lọc theo từ khóa Tìm kiếm
    if (!string.IsNullOrEmpty(searchString))
    {
        var lowerSearch = searchString.ToLower();
        viewModels = viewModels.Where(d => 
            (d.Title != null && d.Title.ToLower().Contains(lowerSearch)) ||
            (d.AuthorName != null && d.AuthorName.ToLower().Contains(lowerSearch))
        );
    }

    // 2. Lọc theo Trạng thái (Dựa trên trường IsApproved mới thêm)
    switch (status.ToLower())
    {
        case "pending":
            viewModels = viewModels.Where(d => !d.IsApproved);
            break;
        case "approved":
            viewModels = viewModels.Where(d => d.IsApproved);
            break;
    }

    // Sắp xếp bài mới nhất lên đầu
    viewModels = viewModels.OrderByDescending(d => d.Id);

    return View(viewModels.ToList());
}

        public async Task<IActionResult> Review(int id)
        {
            var docDto = await _documentService.GetByIdAsync(id);
            if (docDto == null) return NotFound();
            
            var viewModel = _mapper.Map<DocumentViewModel>(docDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _documentService.ApproveDocumentAsync(id);
            if (success) TempData["Success"] = "Đã phê duyệt tài liệu thành công!";
            else TempData["Error"] = "Có lỗi xảy ra khi phê duyệt.";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = true; // Mặc định là true vì đang ở Area Admin

            var success = await _documentService.DeleteAsync(id, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa tài liệu thất bại!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Xóa tài liệu thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
    
}