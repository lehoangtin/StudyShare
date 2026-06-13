<div align="center">

# 📚 StudyShare

**Nền tảng Chia sẻ & Quản lý Tài liệu Học tập Trực tuyến**

[![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQLServer-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

*Môi trường học tập kết nối sinh viên, dễ dàng trao đổi tài liệu và kiến thức.*

</div>

---

## 🌟 Giới thiệu

**StudyShare** là một ứng dụng web giúp người dùng (đặc biệt là học sinh, sinh viên) dễ dàng upload, tìm kiếm, lưu trữ và quản lý các tài liệu học tập. Dự án được xây dựng với mục tiêu tạo ra một thư viện mở, nơi mọi người có thể lan tỏa tri thức một cách tiện lợi và trực quan nhất.

## ✨ Tính năng nổi bật

- 🗂️ **Quản lý danh mục & tài liệu:** Tổ chức tài liệu khoa học theo các chuyên ngành, môn học.
- 🔍 **Tìm kiếm thông minh:** Tìm kiếm tài liệu nhanh chóng theo tên, tác giả hoặc từ khóa.
- 🛡️ **Hệ thống kiểm duyệt & Báo cáo:** Quản trị viên có thể xem xét các báo cáo vi phạm và tự động cộng/trừ điểm uy tín của người dùng.
- 📱 **Giao diện Responsive:** Trải nghiệm mượt mà trên cả máy tính lẫn điện thoại với thiết kế rộng rãi (Max-width 1600px).
- 👤 **Hồ sơ cá nhân:** Theo dõi lịch sử chia sẻ, điểm số và các tài liệu đã lưu.

---

## 💻 Công nghệ sử dụng

Dự án được phát triển dựa trên hệ sinh thái mạnh mẽ của Microsoft và các công nghệ web hiện đại:

- **Back-end:** `.NET 8` (hoặc phiên bản tương ứng), `C#`
- **Database:** `SQL Server`, `Entity Framework Core` (Code-First Migrations)
- **Front-end:** `HTML5`, `CSS3` (Giao diện tùy chỉnh), `JavaScript`, Razor Pages / MVC
- **Tools:** Visual Studio / VS Code, Git, Azure (Tuỳ chọn Deploy)

---

## 🚀 Hướng dẫn cài đặt & Chạy dự án (Local)

Làm theo các bước sau để thiết lập dự án trên máy của bạn:

### 1. Yêu cầu hệ thống (Prerequisites)
- Đã cài đặt [.NET SDK](https://dotnet.microsoft.com/download).
- Đã cài đặt [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) hoặc SQL Server Express.
- (Khuyên dùng) IDE: Visual Studio 2022 hoặc Visual Studio Code.

### 2. Cài đặt chi tiết

```bash
# Clone dự án về máy
git clone https://github.com/lehoangtin/StudyShare.git

# Di chuyển vào thư mục dự án
cd StudyShare

# Khôi phục các gói nuget
dotnet restore

# Cập nhật cơ sở dữ liệu (Áp dụng các file Migration)
dotnet ef database update

# Chạy ứng dụng
dotnet run
```
*Lưu ý: Bạn có thể cần cấu hình lại chuỗi kết nối (Connection String) trong file `appsettings.json` để phù hợp với database SQL Server trên máy của bạn.*

---

## 🤝 Đóng góp (Contributing)

Chúng tôi luôn hoan nghênh những đóng góp từ cộng đồng để làm cho StudyShare ngày càng tốt hơn!
1. Fork dự án.
2. Tạo một nhánh mới (`git checkout -b feature/AmazingFeature`).
3. Commit các thay đổi (`git commit -m 'Add some AmazingFeature'`).
4. Push lên nhánh vừa tạo (`git push origin feature/AmazingFeature`).
5. Tạo một **Pull Request** và chờ đợi review.

---

<div align="center">
  <b>Được phát triển với ❤️ bởi đội ngũ StudyShare</b>
</div>
