using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyShare.DTOs.Requests;
using StudyShare.Models;
using StudyShare.Services.Interfaces;
using StudyShare.ViewModels;
using System.Security.Claims;

namespace StudyShare.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public DocumentController(
            IDocumentService documentService, 
            IUserService userService, 
            UserManager<AppUser> userManager, 
            ICategoryService categoryService, 
            IWebHostEnvironment webHostEnvironment, 
            IMapper mapper)
        {
            _documentService = documentService;
            _userService = userService;
            _userManager = userManager;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var dtoList = await _documentService.GetUserDocumentsAsync(currentUserId);
            var viewModels = _mapper.Map<IEnumerable<DocumentViewModel>>(dtoList);

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _documentService.GetDocumentDetailsAsync(id);
            if (data == null) return NotFound();
            
            var viewModel = _mapper.Map<DocumentViewModel>(data);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                return View(viewModel);
            }

            var request = _mapper.Map<DocumentCreateRequest>(viewModel);
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await _documentService.CreateAsync(request, currentUserId);
            
            TempData["Success"] = "Tải lên thành công! Tài liệu đang chờ duyệt.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var requestDto = await _documentService.GetForEditAsync(id);
            if (requestDto == null) return NotFound();

            var viewModel = _mapper.Map<DocumentEditViewModel>(requestDto);

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", viewModel.CategoryId);

            var existingDocument = await _documentService.GetByIdAsync(id);
            ViewBag.CurrentFileName = existingDocument?.FileName ?? "Không có tệp";

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", viewModel.CategoryId);
                
                var existingDocument = await _documentService.GetByIdAsync(viewModel.Id);
                ViewBag.CurrentFileName = existingDocument?.FileName ?? "Không có tệp";
                return View(viewModel);
            }

            var request = _mapper.Map<DocumentUpdateRequest>(viewModel);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            bool isAdmin = User.IsInRole("Admin");

            var success = await _documentService.UpdateAsync(request, currentUserId, isAdmin);
            if (!success) return Unauthorized("Bạn không có quyền chỉnh sửa tài liệu này hoặc tài liệu không tồn tại.");

            TempData["Success"] = "Cập nhật tài liệu thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var success = await _documentService.DeleteAsync(id, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa thất bại! Bạn không có quyền xóa tài liệu này hoặc tài liệu không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Xóa tài liệu thành công. Điểm của bạn đã được cập nhật!";
            return RedirectToAction(nameof(Index));
        }

[HttpGet]
public async Task<IActionResult> Download(int id)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(currentUserId)) return Challenge();

    var document = await _documentService.GetByIdAsync(id);
    var user = await _userService.GetUserProfileAsync(currentUserId);

    if (document == null || user == null) return NotFound("Không tìm thấy tài liệu hoặc người dùng.");

    int downloadCost = 10;
    bool isFree = User.IsInRole("Admin") || document.UserId == currentUserId;

    if (!isFree)
    {
        if (user.Points < downloadCost) 
        {
            TempData["Error"] = $"Bạn không đủ điểm để tải tài liệu này (Cần {downloadCost} điểm, bạn đang có {user.Points} điểm).";
            return RedirectToAction("Details", new { id = id });
        }

        // Trừ điểm người dùng (Dùng AddPointsAsync với số âm để tránh nhầm lẫn với việc phạt vi phạm)
        await _userService.AddPointsAsync(currentUserId, -downloadCost);
    }

    await _documentService.IncreaseDownloadCountAsync(id);

    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
    
    if (!System.IO.File.Exists(physicalPath))
    {
        TempData["Error"] = "Tệp tin không tồn tại trên hệ thống.";
        return RedirectToAction("Details", new { id = id });
    }

    byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
    
    // --- ĐOẠN ĐÃ SỬA LỖI MIME TYPE ---
    // Sử dụng Provider của ASP.NET Core để tự động dịch đuôi file (.pdf, .docx) sang chuẩn MIME Type (application/pdf,...)
    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
    
    // Kiểm tra xem hệ thống có nhận diện được đuôi file này không
    if (!provider.TryGetContentType(document.FileName, out var contentType))
    {
        // Nếu đuôi file lạ, ép về kiểu mặc định để trình duyệt tự tải xuống
        contentType = "application/octet-stream";
    }

    // Trả về file với Content-Type chuẩn (ví dụ: "application/pdf")
    return File(fileBytes, contentType, document.FileName);
}
// redirect ve mydocuments sau khi xoa de nguoi dung de dang quan ly tai lieu cua minh hon, thay vi redirect ve index co the co nhieu tai lieu khac cua nguoi dung khac
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            var success = await _documentService.DeleteAsync(id, userId, false);
            if (success) TempData["Success"] = "Tài liệu của bạn đã được xóa vĩnh viễn.";
            else TempData["Error"] = "Có lỗi xảy ra hoặc bạn không có quyền xóa.";
            
            return RedirectToAction(nameof(MyDocuments));
        }

        [HttpGet]
        public async Task<IActionResult> MyDocuments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            ViewBag.CurrentUser = await _userManager.FindByIdAsync(userId);
            
            var dtoList = await _documentService.GetUserDocumentsAsync(userId);
            var viewModels = _mapper.Map<IEnumerable<DocumentViewModel>>(dtoList);
            
            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDocument(int docId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _userService.SaveDocumentAsync(userId, docId);
            if (success) TempData["Success"] = "Đã lưu tài liệu vào danh sách của bạn!";
            
            return RedirectToAction("ViewDocument", "Home", new { area = "", id = docId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsaveDocument(int docId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _userService.UnsaveDocumentAsync(userId, docId);
            if (success) TempData["Success"] = "Đã bỏ lưu tài liệu.";

            string returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("SavedDocuments"))
            {
                return RedirectToAction(nameof(SavedDocuments));
            }
            return RedirectToAction("ViewDocument", "Home", new { area = "", id = docId });
        }

        [HttpGet]
        public async Task<IActionResult> SavedDocuments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            ViewBag.CurrentUser = await _userManager.FindByIdAsync(userId);
            
            var savedDocsList = await _userService.GetSavedDocumentsAsync(userId);
            
            var documents = savedDocsList.Select(s => s.Document).ToList();
            var viewModels = _mapper.Map<IEnumerable<DocumentViewModel>>(documents);
            
            return View(viewModels);
        }
    }
}