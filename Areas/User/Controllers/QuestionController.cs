using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using System.Security.Claims;
using AutoMapper;
using StudyShare.ViewModels;
using StudyShare.Models;

namespace StudyShare.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService; 
        private readonly IMapper _mapper;
        private readonly IAIService _aiService; 
        private readonly IUserService _userService;
        private readonly IReportService _reportService;
        
        private readonly UserManager<AppUser> _userManager;

        public QuestionController(
            IQuestionService questionService, 
            IAnswerService answerService, 
            IMapper mapper, 
            IAIService aiService, 
            IUserService userService,
            IReportService reportService,
            UserManager<AppUser> userManager)
        {
            _questionService = questionService;
            _answerService = answerService;
            _mapper = mapper;
            _aiService = aiService;
            _userService = userService;
            _reportService = reportService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
{
    // Lưu lại từ khóa tìm kiếm để hiển thị lại trên ô input sau khi tải trang
    ViewData["CurrentFilter"] = searchString;

    var questionsDto = await _questionService.GetAllAsync();
    var viewModels = _mapper.Map<IEnumerable<QuestionViewModel>>(questionsDto);

    // Thực hiện lọc nếu có từ khóa
    if (!string.IsNullOrEmpty(searchString))
    {
        searchString = searchString.ToLower();
        viewModels = viewModels.Where(q => 
            (q.Content != null && q.Content.ToLower().Contains(searchString))
        );
    }

    // Sắp xếp câu hỏi mới nhất lên đầu
    viewModels = viewModels.OrderByDescending(q => q.CreatedAt);

    return View(viewModels);
}

        public async Task<IActionResult> Details(int id)
        {
            var questionDto = await _questionService.GetByIdAsync(id);
            if (questionDto == null) return NotFound();

            var viewModel = _mapper.Map<QuestionViewModel>(questionDto);
            var answers = await _answerService.GetByQuestionIdAsync(id);
            viewModel.Answers = _mapper.Map<IEnumerable<AnswerViewModel>>(answers).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionCreateViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            
            var request = _mapper.Map<QuestionCreateRequest>(viewModel);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var aiCheck = await _aiService.CheckContentAsync(request.Content);
            if (aiCheck.isFlagged)
            {
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);

                // Gọi qua ReportService
                await _reportService.CreateAutoReportAsync(currentUserId, aiCheck.reason, "Hệ thống AI đã tự động chặn và xử phạt (Trừ 10 điểm, 1 gậy cảnh cáo).");

                TempData["Error"] = $"Nội dung vi phạm: {aiCheck.reason}. Bạn bị trừ 10 điểm và nhận 1 gậy cảnh cáo.";
                return View(viewModel);
            }

            await _questionService.CreateAsync(request, currentUserId);

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user != null) 
            {
                user.Points += 5; 
                await _userManager.UpdateAsync(user);
            }

            TempData["Success"] = "Đăng câu hỏi thành công! Bạn được cộng 5 điểm.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (question.UserId != currentUserId && !User.IsInRole("Admin"))
                return Unauthorized("Bạn không có quyền sửa câu hỏi này.");

            var viewModel = new QuestionEditViewModel { Id = question.Id, Content = question.Content };
            return View(viewModel);
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(QuestionEditViewModel viewModel)
{
    if (!ModelState.IsValid) return View(viewModel);

    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    bool isAdmin = User.IsInRole("Admin");

    // --- 1. CHÈN KIỂM DUYỆT AI VÀO ĐÂY ---
    // Kiểm tra cả Tiêu đề và Nội dung mới xem có vi phạm không
    var aiCheckContent = await _aiService.CheckContentAsync(viewModel.Content);

    if (aiCheckContent.isFlagged)
    {
        string reason = aiCheckContent.reason;
        
        // Phạt người dùng vì cố tình sửa nội dung vi phạm
        await _userService.PenalizeUserAsync(currentUserId, 10, 1);
        
        TempData["Error"] = $"Nội dung chỉnh sửa vi phạm: {reason}. Bạn bị trừ 10 điểm và nhận 1 gậy cảnh cáo.";
        
        // Trả về View Edit để họ sửa lại cho đúng
        return View(viewModel);
    }
    // -------------------------------------

    var request = _mapper.Map<QuestionUpdateRequest>(viewModel);
    var success = await _questionService.UpdateAsync(request, currentUserId, isAdmin);

    if (success)
    {
        TempData["Success"] = "Cập nhật câu hỏi thành công!";
        return RedirectToAction(nameof(Details), new { id = viewModel.Id });
    }

    TempData["Error"] = "Bạn không có quyền chỉnh sửa hoặc có lỗi xảy ra.";
    return RedirectToAction(nameof(Index));
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           if (string.IsNullOrEmpty(currentUserId)) return Unauthorized(); // Fix lỗi null CS8604
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var success = await _questionService.DeleteAsync(id, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa thất bại! Bạn không có quyền xóa câu hỏi này.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Đã xóa câu hỏi thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnswer(AnswerCreateViewModel viewModel)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = viewModel.QuestionId });

            var request = _mapper.Map<AnswerCreateRequest>(viewModel);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var aiCheck = await _aiService.CheckContentAsync(request.Content);
            if (aiCheck.isFlagged)
            {
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);
                
                // Gọi qua ReportService
                await _reportService.CreateAutoReportAsync(currentUserId, aiCheck.reason, "Hệ thống AI đã tự động chặn và xử phạt (Trừ 10 điểm, 1 gậy cảnh cáo).", qid: request.QuestionId);
                
                TempData["Error"] = $"Bình luận vi phạm: {aiCheck.reason}. Bạn bị trừ 10 điểm.";
                return RedirectToAction(nameof(Details), new { id = request.QuestionId });
            }

            var success = await _answerService.CreateAsync(request, currentUserId);
            if (success) 
            {
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null) 
                {
                    user.Points += 3;
                    await _userManager.UpdateAsync(user);
                }
                TempData["Success"] = "Đã đăng câu trả lời! Bạn được cộng 3 điểm."; 
            }
            
            return RedirectToAction(nameof(Details), new { id = request.QuestionId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Cần truyền vào 2 tham số: ID của câu trả lời muốn xóa, và ID của câu hỏi để quay về
        public async Task<IActionResult> DeleteAnswer(int answerId, int questionId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized(); // Fix lỗi null CS8604
            bool isAdmin = User.IsInRole("Admin");

            // Gọi Service xử lý xóa và trừ điểm
            var success = await _answerService.DeleteAsync(answerId, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa câu trả lời thất bại! Bạn không có quyền hoặc câu trả lời không tồn tại.";
            }
            else
            {
                TempData["Success"] = "Xóa câu trả lời thành công. Điểm của bạn đã được cập nhật!";
            }

            // Quay lại trang chi tiết của câu hỏi
            return RedirectToAction(nameof(Details), new { id = questionId });
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Report(int? questionId, int? answerId, string reason)
{
    var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(reporterId)) return Unauthorized();

    string targetUserId = "";
    int redirectQuestionId = 0; // Biến lưu sẵn ID để redirect về trang Details an toàn

    // Nếu là report câu trả lời
    if (answerId.HasValue)
    {
        var answer = await _answerService.GetByIdAsync(answerId.Value);
        if (answer != null) 
        {
            targetUserId = answer.UserId;
            redirectQuestionId = answer.QuestionId;
        }
    }
    // Nếu là report câu hỏi
    else if (questionId.HasValue)
    {
        var question = await _questionService.GetByIdAsync(questionId.Value);
        if (question != null) 
        {
            targetUserId = question.UserId;
            redirectQuestionId = question.Id;
        }
    }

    // Kiểm tra tính hợp lệ (Không được tự report chính mình)
    if (string.IsNullOrEmpty(targetUserId) || reporterId == targetUserId)
    {
        TempData["Error"] = "Thao tác không hợp lệ.";
        return RedirectToAction("Details", new { id = redirectQuestionId });
    }

    // Lưu Report thông qua Service
    await _reportService.CreateReportAsync(reporterId, targetUserId, reason, questionId, answerId);

    TempData["Success"] = "Cảm ơn bạn! Báo cáo đã được gửi tới Quản trị viên.";
    
    // Trả về đúng trang chi tiết câu hỏi một cách an toàn tuyệt đối
    return RedirectToAction("Details", new { id = redirectQuestionId });
}
        public async Task<IActionResult> MyQuestions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            ViewBag.CurrentUser = await _userManager.FindByIdAsync(userId);
            
            // Lấy qua Service và map qua ViewModel
            var dtoList = await _questionService.GetUserQuestionsAsync(userId); 
            var viewModels = _mapper.Map<IEnumerable<QuestionViewModel>>(dtoList);
            
            return View(viewModels);
        }
        // redirect về MyQuestions sau khi xóa để dễ dàng quản lý lại các câu hỏi còn lại của mình
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] // Nên thêm ValidateAntiForgeryToken cho các action POST xóa dữ liệu
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // SỬA Ở ĐÂY: Đổi thành DeleteAsync và truyền 'false' cho isAdmin
            var success = await _questionService.DeleteAsync(id, userId, false);
            
            if (success) TempData["Success"] = "Đã xóa thảo luận và các dữ liệu liên quan thành công.";
            else TempData["Error"] = "Có lỗi xảy ra hoặc bạn không có quyền xóa.";
            
            return RedirectToAction(nameof(MyQuestions));
        }
    }
}