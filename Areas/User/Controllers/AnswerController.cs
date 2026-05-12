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
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AnswerController(IAnswerService answerService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _answerService = answerService;
            _userManager = userManager;
            _mapper = mapper;
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
        public async Task<IActionResult> Delete(int answerId, int questionId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            var success = await _answerService.DeleteAsync(answerId, currentUserId, isAdmin);

            if (!success)
            {
                TempData["Error"] = "Xóa câu trả lời thất bại! Bạn không có quyền hoặc câu trả lời không tồn tại.";
            }
            else
            {
                TempData["Success"] = "Xóa câu trả lời thành công. Điểm của bạn đã được cập nhật!";
            }

            return RedirectToAction("Details", "Question", new { id = questionId });
        }
        [HttpGet]
        public async Task<IActionResult> MyAnswers()
        {
            // Lấy ID của người dùng hiện tại đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var myAnswers = await _answerService.GetByUserIdAsync(userId);
            
            return View(myAnswers);
        }
    }
}