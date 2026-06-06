using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyShare.Models;
using StudyShare.Services;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using StudyShare.Services.Implementations;
using StudyShare.Mappings;
using StudyShare.Repositories.Interfaces;
using StudyShare.Repositories.Implementations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 🔑 Persist Data Protection keys vào disk để không bị mất khi Docker restart
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/dataprotection-keys"))
    .SetApplicationName("StudyShare");

// 🔥 AI Service (giữ từ nhánh minh)
// builder.Services.AddHttpClient<ai.Services.AIService>();
builder.Services.AddHttpClient<IAIService, AIService>();
// Database
builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PBL3ConnectionString")
    ));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
// builder.Services.AddHttpClient<ai.Services.AIService>();
// 2. Đăng ký AIService
// Vì AIService của bạn có tiêm HttpClient ở hàm tạo (constructor), 
// nên dùng AddHttpClient là chuẩn nhất trong .NET:
// builder.Services.AddHttpClient<IAIService, AIService>();
builder.Services.AddAutoMapper(cfg => 
{
    // Chỉ định chính xác file MappingProfile của bạn
    cfg.AddProfile<StudyShare.Mappings.MappingProfile>();
});
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAuthService, AuthService>(); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie (giữ 1 cái thôi)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

// Mail
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<EmailSender>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    int retries = 0;
    bool connected = false;

    // Chờ tối đa 100 giây (20 lần * 5 giây)
    while (!connected && retries < 20)
    {
        try
        {
            Console.WriteLine($"Đang đợi Database... (lần {retries + 1}/20)");
            if (context.Database.CanConnect())
            {
                connected = true;
                Console.WriteLine("Kết nối Database thành công!");
            }
            else
            {
                throw new Exception("Chưa kết nối được");
            }
        }
        catch (Exception)
        {
            retries++;
            System.Threading.Thread.Sleep(5000); // Đợi 5 giây trước khi thử lại
        }
    }

    if (connected)
    {
        try {
            Console.WriteLine("Đang chạy Migrate và Seed...");
            await context.Database.MigrateAsync();
            await DataSeeder.SeedAllAsync(services);
            Console.WriteLine("Hoàn tất!");
        } catch (Exception ex) {
            Console.WriteLine("LỖI MIGRATION: " + ex.Message);
        }
    }
}

// 🔀 Cho phép đọc đúng Host/Scheme khi chạy sau Docker
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// app.MapRazorPages();
// Areas route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();