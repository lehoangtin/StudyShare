using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudyShare.Models;
using System;
using System.Collections.Generic;
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
                ("lehoangtin@gmail.com", "Lê Hoàng Tín", 500, 0, false, new[] { "Admin" }),
                ("sinhvien1@gmail.com", "Ngô Gia Toàn", 200, 0, false, new[] { "User" }),
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
                        CreatedAt = DateTime.Now // Thêm trường thời gian tạo
                    };
                    
                    var result = await userManager.CreateAsync(user, "User@123");
                    if (result.Succeeded)
                    {
                        foreach (var r in u.Roles)
                        {
                            await userManager.AddToRoleAsync(user, r);
                        }
                    }
                }
            }

            // ==============================================================
            // 3. KHỞI TẠO THÊM 15 SINH VIÊN (CHO CÓ DATA ĐỂ TEST)
            // ==============================================================
            for (int i = 3; i <= 17; i++)
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
                        Points = new Random().Next(10, 300), // Cho điểm ngẫu nhiên
                        CreatedAt = DateTime.Now
                    };
                    
                    var result = await userManager.CreateAsync(user, "User@123");
                    if (result.Succeeded) 
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // (Không gõ thêm logic tạo Document, Question... ở đây. Để T-SQL chạy nhanh hơn)
        }
    }
}