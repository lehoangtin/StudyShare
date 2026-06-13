using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyShare.DTOs.Responses;
using StudyShare.Services.Interfaces;
using StudyShare.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace StudyShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IReportService _reportService;
        private readonly IQuestionService _questionService; // Bổ sung QuestionService
        private readonly IAnswerService _answerService; // Bổ sung AnswerService
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IReportService reportService, IQuestionService questionService, IAnswerService answerService, IDocumentService documentService, IMapper mapper)
        {
            _userService = userService;
            _reportService = reportService;
            _questionService = questionService;
            _answerService = answerService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchString)
{
    ViewData["CurrentFilter"] = searchString;

    // 1. Lấy dữ liệu từ lớp Service (Trả về IEnumerable<UserResponse>)
    var users = await _userService.GetAllUsersAsync();

    // 2. Map sang ViewModel để hiển thị ra View
    var viewModels = _mapper.Map<IEnumerable<UserViewModel>>(users);

    // 3. Thực hiện lọc dữ liệu
    if (!string.IsNullOrEmpty(searchString))
    {
        searchString = searchString.ToLower();
        viewModels = viewModels.Where(u => 
            (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(searchString)) ||
            (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchString))
        ).ToList(); // Ép kiểu về List để tránh lỗi truy vấn khi truyền vào View
    }

    return View(viewModels);
}
public async Task<IActionResult> Details(string id)
{
    var user = await _userService.GetUserProfileAsync(id);
    if (user == null) return NotFound();

    var viewModel = _mapper.Map<UserViewModel>(user);

    // 1. Lấy lịch sử vi phạm (Bắt buộc phải có đoạn này thì bảng lịch sử mới hiện)
    var reportsDto = await _reportService.GetReportsForUserAsync(id);
    var violations = _mapper.Map<IEnumerable<ReportViewModel>>(reportsDto)
                            .Where(r => !string.IsNullOrEmpty(r.ActionTaken))
                            .OrderByDescending(r => r.CreatedAt);
    ViewBag.Violations = violations;

    return View(viewModel);
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBan(string id)
        {
            var result = await _userService.ToggleBanUserAsync(id);
            if (result) TempData["Success"] = "Đã cập nhật trạng thái hoạt động của tài khoản.";
            else TempData["Error"] = "Cập nhật trạng thái thất bại. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ReportedUsers()
        {
            var usersDto = await _userService.GetReportedUsersAsync();
            var viewModels = _mapper.Map<IEnumerable<UserViewModel>>(usersDto);
            return View(viewModels);
        }

        // Đã chuẩn tham số ID để load chi tiết hồ sơ
        public async Task<IActionResult> ViewReports(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("Không tìm thấy ID người dùng.");

            var reportsDto = await _reportService.GetReportsForUserAsync(id);
            var viewModels = _mapper.Map<IEnumerable<ReportViewModel>>(reportsDto);
            
            ViewBag.TargetUserId = id; 
            
            // Lấy thông tin user để hiển thị tên trên View
            var user = await _userService.GetUserProfileAsync(id);
            ViewBag.TargetUser = user?.FullName ?? "Người dùng ẩn danh";

            return View(viewModels);
        }
        // Hỗ trợ giao diện 2 Tab (Dashboard)
        public async Task<IActionResult> PendingReports()
        {
            var pendingDto = await _reportService.GetAllPendingReportsAsync();
            
            var resolvedDto = await _reportService.GetResolvedReportsAsync(); 

            var viewModel = new ReportDashboardViewModel
            {
                PendingReports = _mapper.Map<IEnumerable<ReportViewModel>>(pendingDto),
                ResolvedReports = _mapper.Map<IEnumerable<ReportViewModel>>(resolvedDto)
            };
            return View(viewModel);
        }

        // Dùng hàm PenalizeUserAsync (Trừ 10 điểm, +1 gậy)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Penalize(string userId, int reportId, int pointsDeducted = 10)
        {
        if (string.IsNullOrEmpty(userId)) return NotFound("Không tìm thấy ID người dùng.");

            // 1. Lấy thông tin Report
            var report = await _reportService.GetByIdAsync(reportId);
            if (report == null) return NotFound("Không tìm thấy báo cáo.");

            // 2. XÓA NỘI DUNG BỊ VI PHẠM (Chỉ xử lý Question và Answer)
            bool isContentDeleted = false;
            if (report.QuestionId.HasValue)
            {
                isContentDeleted = await _questionService.DeleteByAdminAsync(report.QuestionId.Value);
            }
            else if (report.AnswerId.HasValue)
            {
                isContentDeleted = await _answerService.DeleteByAdminAsync(report.AnswerId.Value);
            }

            // 3. PHẠT USER: Trừ điểm và Tăng 1 gậy (WarningCount)
            var success = await _userService.PenalizeUserAsync(userId, pointsDeducted, 1);

            // 4. Luôn luôn cập nhật trạng thái Report vào lịch sử, bất kể bước phạt có thành công hay không
            string actionMessage;
            if (success)
            {
                actionMessage = isContentDeleted
                    ? $"Admin đã xóa bài vi phạm, trừ {pointsDeducted} điểm và cảnh cáo."
                    : $"Admin đã trừ {pointsDeducted} điểm và ghi nhận 1 lần cảnh cáo.";
                TempData["Success"] = "Đã xử lý vi phạm, trừ điểm và ghi nhận 1 lần cảnh cáo thành công.";
            }
            else
            {
                actionMessage = "Admin đã xử lý báo cáo nhưng gặp lỗi khi trừ điểm người dùng.";
                TempData["Error"] = "Có lỗi xảy ra khi xử lý user.";
            }
            await _reportService.ResolveWithActionAsync(reportId, actionMessage);
            
            return RedirectToAction(nameof(PendingReports));
        }

        // Bỏ qua báo cáo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DismissReport(int reportId)
        {
            await _reportService.ResolveWithActionAsync(reportId, "Admin đã bỏ qua báo cáo này.");
            TempData["Success"] = "Đã bỏ qua báo cáo.";
            return RedirectToAction("PendingReports");
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _userService.UpdateUserRoleAsync(userId, role, currentUserId);

            if (success) TempData["Success"] = "Cập nhật quyền hạn thành công!";
            else TempData["Error"] = "Bạn không có đủ thẩm quyền thực hiện thao tác này!";

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")] // Chỉ Super Admin mới được vào trang này
        public async Task<IActionResult> ManageRoles(string searchString)
        {
            var users = await _userService.GetAllUsersAsync();
            var viewModels = _mapper.Map<IEnumerable<UserViewModel>>(users);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                viewModels = viewModels.Where(u => 
                    (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(searchString)) || 
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchString))
                ).ToList();
            }

            return View(viewModels);
        }
    }
}