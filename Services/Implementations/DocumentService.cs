using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using StudyShare.Models;
using StudyShare.DTOs.Requests;
using StudyShare.DTOs.Responses;
using StudyShare.Services.Interfaces;
using StudyShare.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudyShare.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentService(IDocumentRepository documentRepository, IUserService userService, IMapper mapper, IWebHostEnvironment webHostEnvironment, AppDbContext context, IUserRepository userRepository)
        {
            _documentRepository = documentRepository;
            _userService = userService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<DocumentResponse>> GetAllAsync()
        {
            var docs = await _documentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
        }

        public async Task<DocumentResponse?> GetByIdAsync(int id)
        {
            var doc = await _documentRepository.GetByIdAsync(id);
            return doc == null ? null : _mapper.Map<DocumentResponse>(doc);
        }

        public async Task<DocumentUpdateRequest?> GetForEditAsync(int id)
        {
            var doc = await _documentRepository.GetForEditAsync(id);
            if (doc == null) return null;

            return new DocumentUpdateRequest
            {
                Id = doc.Id,
                Title = doc.Title,
                Description = doc.Description ?? string.Empty,
                CategoryId = doc.CategoryId
            };
        }

        public async Task<bool> UpdateAsync(DocumentUpdateRequest request, string currentUserId, bool isAdmin)
        {
            var document = await _documentRepository.GetForEditAsync(request.Id);
            if (document == null) return false;
            if (!isAdmin && document.UserId != currentUserId) return false;

            document.Title = request.Title;
            document.Description = request.Description;
            document.CategoryId = request.CategoryId;

            if (request.File != null && request.File.Length > 0)
            {
                var oldPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(oldPhysicalPath))
                {
                    System.IO.File.Delete(oldPhysicalPath);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(request.File.FileName);
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                document.FilePath = "/uploads/" + uniqueFileName;
                document.FileName = request.File.FileName;
                document.FileType = request.File.ContentType;
                document.FileSize = request.File.Length;
            }

            return await _documentRepository.UpdateAsync(document);
        }

        public async Task<bool> CreateAsync(DocumentCreateRequest request, string userId)
        {
            var document = _mapper.Map<Document>(request);
            document.UserId = userId;
            document.UploadDate = DateTime.Now;
            document.IsApproved = false; // Mặc định phải chờ Admin duyệt

            if (request.File != null && request.File.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(request.File.FileName);
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                document.FilePath = "/uploads/" + uniqueFileName;
                document.FileName = request.File.FileName;
                document.FileType = request.File.ContentType;
                document.FileSize = request.File.Length;
            }

            return await _documentRepository.CreateAsync(document);
        }

        // Trong DocumentService.cs
// Trong DocumentService.cs
public async Task<bool> DeleteAsync(int documentId, string currentUserId, bool isAdmin)
{
    // 1. Lấy thông tin tài liệu trước khi xóa
    var document = await _documentRepository.GetByIdAsync(documentId);
    if (document == null) 
    {
        return false;
    }

    // 2. Kiểm tra quyền: Chỉ Admin hoặc chính tác giả mới được xóa
    if (!isAdmin && document.UserId != currentUserId)
    {
        return false;
    }

    // 3. XỬ LÝ TRỪ ĐIỂM CỰC KỲ QUAN TRỌNG
    // Chỉ trừ điểm (thu hồi điểm) NẾU tài liệu NÀY ĐÃ ĐƯỢC ADMIN DUYỆT
    if (document.IsApproved)
    {
        // Thu hồi lại đúng số điểm đã cộng lúc duyệt (Ví dụ lúc duyệt cộng 50 thì giờ trừ 50)
        await _userService.AddPointsAsync(document.UserId, -10);
    }
    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
    if (System.IO.File.Exists(physicalPath))
    {
        System.IO.File.Delete(physicalPath);
    }
    // Nếu document.IsApproved == false (tài liệu đang chờ duyệt), 
    // user chưa nhận được điểm nào nên ta KHÔNG TRỪ GÌ CẢ.

    // 4. Tiến hành xóa tài liệu
    return await _documentRepository.DeleteAsync(document);
}        public async Task<IEnumerable<DocumentResponse>> GetAllForAdminAsync(string search)
        {
            var docs = await _documentRepository.GetAllForAdminAsync(search);
            return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
        }

        public async Task<DocumentResponse?> GetDetailsForReviewAsync(int id)
        {
            return await GetByIdAsync(id); 
        }

        public async Task<bool> ApproveDocumentAsync(int id)
        {
            try
            {
                var doc = await _documentRepository.GetByIdAsync(id);
                
                // 1. Nếu không tìm thấy tài liệu
                if (doc == null) return false;

                // 2. Nếu đã duyệt rồi thì coi như thành công (tránh báo lỗi khó hiểu cho Admin)
                if (doc.IsApproved) return true;

                // 3. Đổi trạng thái và cập nhật qua hàm UpdateAsync chung
                doc.IsApproved = true;
                var success = await _documentRepository.UpdateAsync(doc);
                
                if (success)
                {
                    // 4. Cộng điểm thưởng
                    // (Nếu hàm này lỗi, nó sẽ nhảy xuống catch và không làm crash app)
                    await _userService.AddPointsAsync(doc.UserId, 10);
                }

                return success;
            }
            catch (Exception ex)
            {
                // 5. Bạn có thể log lại ex.Message ở đây hoặc đặt Breakpoint để xem lỗi thực sự là gì
                Console.WriteLine($"Lỗi khi duyệt tài liệu: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> IncreaseDownloadCountAsync(int id)
        {
            var doc = await _documentRepository.GetByIdAsync(id);
            if (doc == null) return false;
            
            doc.DownloadCount++;
            return await _documentRepository.UpdateAsync(doc);
        }
        public async Task<IEnumerable<DocumentResponse>> GetUserDocumentsAsync(string userId)
        {
            var docs = await _documentRepository.GetUserDocumentsAsync(userId);

            return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
        }

        
        public async Task<IEnumerable<DocumentResponse>> GetApprovedDocumentsAsync(string searchTerm, int? categoryId)
        {
            var docs = await _documentRepository.GetApprovedDocumentsAsync(searchTerm, categoryId);
            return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
        }

        public async Task<DocumentResponse?> GetDocumentDetailsAsync(int id)
        {
            var document = await _documentRepository.GetDocumentDetailsAsync(id);
            return document == null ? null : _mapper.Map<DocumentResponse>(document);
        }
        
        public async Task<IEnumerable<DocumentResponse>> GetAllApprovedAsync()
        {
            // 🔥 Tối ưu: Dùng lại hàm GetApprovedDocumentsAsync để lấy trực tiếp từ DB
            // Truyền string rỗng và null để lấy tất cả
            var approvedDocs = await _documentRepository.GetApprovedDocumentsAsync(string.Empty, null);
            return _mapper.Map<IEnumerable<DocumentResponse>>(approvedDocs);
        }
        public async Task<IEnumerable<DocumentResponse>> GetPendingDocumentsAsync()
        {
            // Gọi repo để lấy danh sách chưa duyệt
            var docs = await _documentRepository.GetPendingDocumentsAsync();
            
            // Map từ Entity sang Response DTO để trả về Controller
            return _mapper.Map<IEnumerable<DocumentResponse>>(docs);
        }

    }
}