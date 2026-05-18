using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyShare.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudyShare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")] // Chỉ cho phép Admin và SuperAdmin truy cập
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        // 1. Hiển thị danh sách tất cả bình luận
        public async Task<IActionResult> Index()
        {
            var answers = await _answerService.GetAllForAdminAsync();
            return View(answers);
        }

        // 2. Chức năng Xóa bình luận cho Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Lấy ID của Admin đang đăng nhập
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Dùng chung hàm DeleteAsync của Service, truyền true vào tham số isAdmin
            var success = await _answerService.DeleteAsync(id, currentAdminId, true);

            if (success)
            {
                TempData["Success"] = "Đã xóa câu trả lời và trừ điểm người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Xóa câu trả lời thất bại!";
            }
            
            // Xóa xong thì load lại đúng trang Index (Danh sách bình luận)
            return RedirectToAction(nameof(Index));
        }
    }
}