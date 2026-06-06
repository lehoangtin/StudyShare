using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAnswerService _answerService; // Giữ lại để load Answers trong trang Details
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
            ViewData["CurrentFilter"] = searchString;

            var questionsDto = await _questionService.GetAllAsync();
            var viewModels = _mapper.Map<IEnumerable<QuestionViewModel>>(questionsDto);

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                viewModels = viewModels.Where(q => 
                    (q.Content != null && q.Content.ToLower().Contains(searchString))
                );
            }

            viewModels = viewModels.OrderByDescending(q => q.CreatedAt);

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var questionDto = await _questionService.GetByIdAsync(id);
            if (questionDto == null) return NotFound();

            var viewModel = _mapper.Map<QuestionViewModel>(questionDto);
            
            // Vẫn cần gọi IAnswerService ở đây để hiển thị danh sách câu trả lời
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
                await _reportService.CreateAutoReportAsync(currentUserId, aiCheck.reason, "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.", null, null, request.Content);
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);

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

            var aiCheckContent = await _aiService.CheckContentAsync(viewModel.Content);
            if (aiCheckContent.isFlagged)
            {
                await _reportService.CreateAutoReportAsync(currentUserId, aiCheckContent.reason, "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.", null, null, viewModel.Content);
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);
                
                TempData["Error"] = $"Nội dung chỉnh sửa vi phạm: {aiCheckContent.reason}. Bạn bị trừ 10 điểm và nhận 1 gậy cảnh cáo.";
                return View(viewModel);
            }

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
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized(); 
            
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
        public async Task<IActionResult> Report(int? questionId, string reason)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reporterId)) return Unauthorized();

            if (!questionId.HasValue)
            {
                TempData["Error"] = "Thao tác không hợp lệ.";
                return RedirectToAction("Index");
            }

            var question = await _questionService.GetByIdAsync(questionId.Value);
            if (question == null || reporterId == question.UserId)
            {
                TempData["Error"] = "Thao tác không hợp lệ hoặc bạn không thể tự báo cáo chính mình.";
                return RedirectToAction("Details", new { id = questionId.Value });
            }

            await _reportService.CreateReportAsync(reporterId, question.UserId, reason, questionId, null, question.Content);

            TempData["Success"] = "Cảm ơn bạn! Báo cáo đã được gửi tới Quản trị viên.";
            return RedirectToAction("Details", new { id = questionId.Value });
        }

        public async Task<IActionResult> MyQuestions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            
            ViewBag.CurrentUser = await _userManager.FindByIdAsync(userId);
            
            var dtoList = await _questionService.GetUserQuestionsAsync(userId); 
            var viewModels = _mapper.Map<IEnumerable<QuestionViewModel>>(dtoList);
            
            return View(viewModels);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _questionService.DeleteAsync(id, userId, false);
            
            if (success) TempData["Success"] = "Đã xóa thảo luận và các dữ liệu liên quan thành công.";
            else TempData["Error"] = "Có lỗi xảy ra hoặc bạn không có quyền xóa.";
            
            return RedirectToAction(nameof(MyQuestions));
        }
    }
}