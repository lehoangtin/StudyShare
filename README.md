<div align="center">
  <img src="https://img.icons8.com/color/120/000000/knowledge-sharing.png" alt="StudyShare Logo" width="100"/>
  
  # 📚 StudyShare - Nền tảng Chia sẻ Tài liệu & Hỏi đáp
  
  **Đồ án PBL3 - Đại học Bách khoa Đà Nẵng**
  
  Nền tảng hỗ trợ sinh viên chia sẻ tài liệu học tập, thảo luận kiến thức và tương tác với hệ thống quản lý nội dung thông minh.

  <p>
    <img src="https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Core" />
    <img src="https://img.shields.io/badge/Entity%20Framework-3db4d9?style=for-the-badge&logo=.net&logoColor=white" alt="EF Core" />
    <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server" />
    <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
    <img src="https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" alt="Bootstrap" />
  </p>
</div>

---

## 🚀 Tính năng nổi bật (Key Features)

### 🧑‍🎓 Dành cho Người dùng (User)
* **Quản lý Tài khoản:** Đăng nhập, đăng ký, cập nhật hồ sơ cá nhân và đổi mật khẩu an toàn.
* **Chia sẻ Tài liệu:** Tải lên, tải xuống và lưu trữ các tài liệu học tập theo từng danh mục.
* **Hỏi đáp & Thảo luận:** Tạo câu hỏi mới, bình luận/trả lời các thắc mắc của sinh viên khác.
* **Quản lý Cá nhân:** Trung tâm theo dõi "Tài liệu của tôi", "Câu hỏi của tôi", "Câu trả lời của tôi" và các tài liệu đã lưu.

### 🛡️ Dành cho Quản trị viên (Admin)
* **Kiểm duyệt Nội dung:** Duyệt tài liệu trước khi hiển thị công khai.
* **Quản lý Danh mục:** Thêm, sửa, xóa các danh mục tài liệu/môn học.
* **Xử lý Vi phạm:** Tích hợp hệ thống AI nhận diện vi phạm, quản lý báo cáo (Report) từ người dùng và xử phạt (trừ điểm, cảnh cáo).
* **Quản lý Dữ liệu:** Quản lý toàn bộ bài viết, bình luận và người dùng trên nền tảng.

---

## 🏗️ Kiến trúc Hệ thống (Architecture)

Dự án được xây dựng dựa trên **Mô hình 3 lớp (3-Tier Architecture)** chuẩn mực để đảm bảo tính mở rộng và dễ bảo trì:
1. **GUI (Presentation Layer):** Giao diện người dùng sử dụng ASP.NET Core (Razor/Scaffolding), HTML/CSS, Bootstrap và SweetAlert2.
2. **BLL (Business Logic Layer):** Xử lý nghiệp vụ, tính toán điểm số, phân quyền và kiểm duyệt (Services, DTOs, AutoMapper).
3. **DAL (Data Access Layer):** Giao tiếp với cơ sở dữ liệu SQL Server thông qua Entity Framework Core (Repositories).

---

## ⚙️ Cài đặt & Chạy dự án (Installation)

### Yêu cầu hệ thống:
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (Hoặc phiên bản nhóm đang dùng)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* SQL Server Management Studio (SSMS) hoặc Azure Data Studio

### Các bước khởi chạy:

1. **Clone repository:**
   ```bash
   git clone https://github.com/lehoangtin/StudyShare.git
   cd StudyShare
