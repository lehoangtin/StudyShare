using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudyShare.ViewModels;
using StudyShare.Services.Interfaces; 
using StudyShare.Services; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StudyShare.Models;
using Microsoft.Extensions.Logging;
namespace StudyShare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, IEmailSender emailSender, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _authService = authService;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) 
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null) 
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _authService.GetUserByEmailAsync(model.Email); 
                
                if (user != null && user.WarningCount >= 3)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa vĩnh viễn do vi phạm quy định cộng đồng từ 3 lần trở lên.");
                    return View(model);
                }

                var result = await _authService.LoginAsync(model);
                
                if (result.Succeeded)
                {
                    return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) 
                        ? Redirect(returnUrl) 
                        : RedirectToAction("Index", "Home");
                }
                
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản bị khóa tạm thời do nhập sai nhiều lần. Vui lòng thử lại sau 5 phút.");
                    return View(model);
                }

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn chưa được xác nhận. Vui lòng kiểm tra Email để kích hoạt tài khoản trước khi đăng nhập!");
                    return View(model);
                }
                
                ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
            }
            return View(model);
        }
                [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model);
                
                if (result.Succeeded)
                {
                    var user = await _authService.GetUserByEmailAsync(model.Email);
                    if (user != null) 
                    {
                        var code = await _authService.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                            
                        await _emailSender.SendEmailAsync(model.Email, "Xác nhận địa chỉ email của bạn", $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{callbackUrl}'>bấm vào đây</a>.");
                    }

                    TempData["Success"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản.";
                    return RedirectToAction("Login");
                }
                
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
        {
            if (userId == null || code == null) return RedirectToAction("Index", "Home");

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null) return NotFound($"Không tìm thấy người dùng với ID '{userId}'.");

            var result = await _authService.ConfirmEmailAsync(user, code);
            return result.Succeeded ? View("ConfirmSuccess") : View("ConfirmFail");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.GetUserByEmailAsync(model.Email);
                if (user != null) 
                {
                    _logger.LogInformation("[FORGOT PASSWORD] Tìm thấy user {Email}, đang tạo token...", model.Email);
                    // 2. Tạo mã token
                    var code = await _authService.GeneratePasswordResetTokenAsync(user);
                    
                    // để khớp với hàm ResetPassword(string email, string code) của Identity
                    var callbackUrl = Url.Action("ResetPassword", "Account", 
                        new { email = model.Email, code = code }, 
                        protocol: HttpContext.Request.Scheme);

                    _logger.LogInformation("[FORGOT PASSWORD] Callback URL: {Url}", callbackUrl);

                    string htmlMessage = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h2 style='color: #2563eb;'>StudyShare</h2>
                            <h3>Khôi phục mật khẩu</h3>
                            <p>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản {model.Email}.</p>
                            <p>Vui lòng click vào nút bên dưới để tiến hành thiết lập mật khẩu mới:</p>
                            <a href='{callbackUrl}' style='display: inline-block; padding: 12px 24px; background-color: #2563eb; color: white; text-decoration: none; border-radius: 6px; font-weight: bold; margin: 20px 0;'>ĐẶT LẠI MẬT KHẨU</a>
                            <p style='color: #6b7280; font-size: 0.9em;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                        </div>";

                    await _emailSender.SendEmailAsync(model.Email, "Khôi phục mật khẩu - StudyShare", htmlMessage);
                }
                else
                {
                    _logger.LogWarning("[FORGOT PASSWORD] Không tìm thấy user với email: {Email}", model.Email);
                }

                // Báo thành công (Dù email có thật hay không để bảo mật)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult ResetPassword(string? email = null, string? code = null) 
        {
            if (code == null || email == null)
            {
                return BadRequest("Cần có mã Token và Email hợp lệ để đổi mật khẩu.");
            }
            
            // Khởi tạo model và gán sẵn Email, Token (code) vào để truyền xuống giao diện
            var model = new ResetPasswordViewModel 
            { 
                Email = email, 
                Token = code 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _authService.GetUserByEmailAsync(model.Email);
            if (user == null) return RedirectToAction(nameof(ResetPasswordConfirmation));

            var result = await _authService.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded) return RedirectToAction(nameof(ResetPasswordConfirmation));

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult ResendEmailConfirmation() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _authService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email xác nhận đã được gửi đi (nếu email tồn tại trong hệ thống).");
                return View(model);
            }

            var code = await _authService.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                
            await _emailSender.SendEmailAsync(model.Email, "Xác nhận địa chỉ email của bạn", $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{callbackUrl}'>nhấn vào đây</a>.");

            ModelState.AddModelError(string.Empty, "Email xác nhận đã được gửi lại thành công.");
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}