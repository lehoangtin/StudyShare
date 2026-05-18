using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using StudyShare.ViewModels;
using StudyShare.Models;

namespace StudyShare.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class AnswerController : Controller
    {
       private readonly IAnswerService _answerService; 
        private readonly IMapper _mapper;
        private readonly IAIService _aiService; 
        private readonly IUserService _userService;
        private readonly IReportService _reportService;
        private readonly UserManager<AppUser> _userManager;

        public AnswerController(
            IAnswerService answerService, 
            IMapper mapper, 
            IAIService aiService, 
            IUserService userService,
            IReportService reportService,
            UserManager<AppUser> userManager)
        {
            _answerService = answerService;
            _mapper = mapper;
            _aiService = aiService;
            _userService = userService;
            _reportService = reportService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm bảo mật chống tấn công CSRF
        public async Task<IActionResult> Create(AnswerCreateViewModel model)
        {
            if (!ModelState.IsValid) 
                return RedirectToAction("Details", "Question", new { id = model.QuestionId });

            var userId = _userManager.GetUserId(User);
            var request = _mapper.Map<AnswerCreateRequest>(model);
            
            var isSuccess = await _answerService.CreateAsync(request, userId);

            if (!isSuccess)
            {
                TempData["Error"] = "Bình luận của bạn vi phạm tiêu chuẩn nội dung. Bạn bị trừ 10 điểm và nhận 1 gậy cảnh cáo!";
                return RedirectToAction("Details", "Question", new { id = model.QuestionId });
            }

            TempData["Success"] = "Đã gửi bình luận thành công!";
            return RedirectToAction("Details", "Question", new { id = model.QuestionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnswerEditViewModel model)
        {
            if (!ModelState.IsValid) 
                return RedirectToAction("Details", "Question", new { id = model.QuestionId });

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var request = _mapper.Map<AnswerUpdateRequest>(model);

            var isSuccess = await _answerService.UpdateAsync(request, currentUserId, isAdmin);

            if (!isSuccess)
            {
                TempData["Error"] = "Bạn cố tình sửa bình luận thành nội dung vi phạm! Hành động bị chặn và bạn đã bị phạt điểm.";
                return RedirectToAction("Details", "Question", new { id = model.QuestionId });
            }

            TempData["Success"] = "Cập nhật bình luận thành công!";
            return RedirectToAction("Details", "Question", new { id = model.QuestionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnswer(int answerId, int questionId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized(); 
            
            bool isAdmin = User.IsInRole("Admin");

            var success = await _answerService.DeleteAsync(answerId, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa câu trả lời thất bại! Bạn không có quyền hoặc câu trả lời không tồn tại.";
            }
            else
            {
                TempData["Success"] = "Xóa câu trả lời thành công. Điểm của bạn đã được cập nhật!";
            }

            return RedirectToAction("Details", "Question", new { id = questionId, area = "User" });
        }
        [HttpGet]
        public async Task<IActionResult> MyAnswers()
        {
            // Lấy ID của người dùng hiện tại đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var myAnswers = await _answerService.GetByUserIdAsync(userId);
            
            return View(myAnswers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnswer(AnswerCreateViewModel viewModel)
        {
            if (!ModelState.IsValid) 
            {
                return RedirectToAction("Details", "Question", new { id = viewModel.QuestionId, area = "User" });
            }

            var request = _mapper.Map<AnswerCreateRequest>(viewModel);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var aiCheck = await _aiService.CheckContentAsync(request.Content);
            if (aiCheck.isFlagged)
            {
                await _reportService.CreateAutoReportAsync(currentUserId, aiCheck.reason, "Hệ thống AI tự động chặn bình luận vi phạm (Trừ 10 điểm, 1 gậy).", qid: request.QuestionId);
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);
                
                TempData["Error"] = $"Bình luận vi phạm: {aiCheck.reason}. Bạn bị trừ 10 điểm.";
                return RedirectToAction("Details", "Question", new { id = request.QuestionId, area = "User" });
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
            
            return RedirectToAction("Details", "Question", new { id = request.QuestionId, area = "User" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int? answerId, string reason)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(reporterId)) return Unauthorized();

            if (!answerId.HasValue)
            {
                TempData["Error"] = "Thao tác không hợp lệ.";
                // Do Report Answer được gọi từ trang Details của Question, nếu lỗi ta nên trả về trang chủ Question
                return RedirectToAction("Index", "Question", new { area = "User" });
            }

            var answer = await _answerService.GetByIdAsync(answerId.Value);
            if (answer == null || reporterId == answer.UserId)
            {
                TempData["Error"] = "Thao tác không hợp lệ hoặc bạn không thể tự báo cáo chính mình.";
                return RedirectToAction("Details", "Question", new { id = answer?.QuestionId ?? 0, area = "User" });
            }

            await _reportService.CreateReportAsync(reporterId, answer.UserId, reason, null, answerId);

            TempData["Success"] = "Cảm ơn bạn! Báo cáo bình luận đã được gửi tới Quản trị viên.";
            
            return RedirectToAction("Details", "Question", new { id = answer.QuestionId, area = "User" });
        }
    }
}