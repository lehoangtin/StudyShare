using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq; // Cần thiết để dùng Where()
using System.Threading.Tasks;

namespace StudyShare.Models
{
    public static class DataSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // ==============================================================
            // 1. KHỞI TẠO ROLES 
            // ==============================================================
            string[] roles = { "SuperAdmin", "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ==============================================================
            // 2. KHỞI TẠO CÁC USER CHỦ CHỐT VỚI DỮ LIỆU ĐẦY ĐỦ
            // ==============================================================
            var coreUsers = new List<(string Email, string Name, int Points, int Warnings, bool IsBanned, string[] Roles)>
            {
                ("superadmin@studyshare.com", "Trùm Cuối", 9999, 0, false, new[] { "SuperAdmin", "Admin" }),
                ("admin@gmail.com", "Quản trị viên 1", 1000, 0, false, new[] { "Admin" }),
                
                // Team Dev (Cấp point cao và quyền Admin để test)
                ("lehoangtin@gmail.com", "Lê Hoàng Tín", 500, 0, false, new[] { "Admin", "User" }),
                ("ngogiatoan@gmail.com", "Ngô Gia Toàn", 500, 0, false, new[] { "Admin", "User" }),
                ("minh@gmail.com", "Minh", 500, 0, false, new[] { "Admin", "User" }),
                ("duong@gmail.com", "Dương", 500, 0, false, new[] { "Admin", "User" }),

                // Các User phục vụ test Case đặc biệt
                ("sinhvien2@gmail.com", "Sinh Viên Chăm Chỉ 2", 200, 0, false, new[] { "User" }),
                ("vipham@gmail.com", "User Bị 3 Gậy", 0, 3, false, new[] { "User" }), 
                ("banned@gmail.com", "User Bị Khóa", -50, 5, true, new[] { "User" })   
            };

            foreach (var u in coreUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(u.Email);
                if (existingUser == null)
                {
                    var user = new AppUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.Name,
                        EmailConfirmed = true,
                        Points = u.Points,
                        WarningCount = u.Warnings,
                        IsBanned = u.IsBanned,
                        CreatedAt = DateTime.Now 
                    };
                    
                    var result = await userManager.CreateAsync(user, "User@123"); // Pass chung cho tiện test
                    if (result.Succeeded)
                    {
                        foreach (var r in u.Roles)
                        {
                            await userManager.AddToRoleAsync(user, r);
                        }
                    }
                }
            }

            // 3. KHỞI TẠO THÊM 50 SINH VIÊN (CHO CÓ DATA ĐỂ TEST PHÂN TRANG)
            var random = new Random();
            for (int i = 1; i <= 50; i++)
            {
                string email = $"student{i}@gmail.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new AppUser 
                    { 
                        UserName = email, 
                        Email = email, 
                        FullName = $"Sinh Viên Thứ {i}", 
                        EmailConfirmed = true,
                        Points = random.Next(10, 500), // Random điểm từ 10 đến 500
                        // Random ngày tạo trong vòng 30 ngày qua để test sort/filter
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30)) 
                    };
                    
                    var result = await userManager.CreateAsync(user, "User@123");
                    if (result.Succeeded) 
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

        }
    }
}