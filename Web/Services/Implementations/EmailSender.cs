using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyShare.Services.Interfaces;

namespace StudyShare.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<MailSettings> mailSettings, ILogger<EmailSender> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
{
    var message = new MimeMessage();
    message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
    message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
    message.To.Add(MailboxAddress.Parse(email));
    message.Subject = subject;

    var builder = new BodyBuilder { HtmlBody = htmlMessage };
    message.Body = builder.ToMessageBody();

    using var smtp = new SmtpClient();
    
    // Bỏ qua kiểm tra chứng chỉ SSL (Quan trọng khi chạy ở Localhost)
    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

    try
    {
        _logger.LogInformation("[EMAIL] Đang kết nối SMTP {Host}:{Port}...", _mailSettings.Host, _mailSettings.Port);
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(message);
        _logger.LogInformation("[EMAIL] Gửi email thành công tới {To}", email);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "[EMAIL] LỖI khi gửi email tới {To}", email);
        throw;
    }
    finally
    {
        await smtp.DisconnectAsync(true);
    }
}}
    // Lớp Model để map dữ liệu từ appsettings.json
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}