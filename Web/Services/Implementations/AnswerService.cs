using AutoMapper;
using StudyShare.Models;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using StudyShare.Repositories.Interfaces;
using StudyShare.DTOs.Responses; 

namespace StudyShare.Services.Implementations
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserService _userService;
        private readonly IAIService _aiService; // Thêm AI Service
        private readonly IReportService _reportService; // Thêm Report Service
        private readonly IMapper _mapper;

        // Đã xóa IUserRepository vì không sử dụng tới
        public AnswerService(
            IAnswerRepository answerRepository, 
            IUserService userService, 
            IAIService aiService, 
            IReportService reportService, 
            IMapper mapper)
        {
            _answerRepository = answerRepository;
            _userService = userService;
            _aiService = aiService;
            _reportService = reportService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(AnswerCreateRequest request, string userId)
        {
            // 1. AI KIỂM TRA NỘI DUNG BÌNH LUẬN
            var contentToCheck = request.Content + " " + request.Content;
            var aiResult = await _aiService.CheckContentAsync(contentToCheck);

            if (aiResult.isFlagged)
            {
                // 2. PHẠT NGƯỜI DÙNG (Trừ 10 điểm, tăng 1 gậy)
                await _userService.PenalizeUserAsync(userId, 10, 1);

                // 3. LƯU VÀO LỊCH SỬ VI PHẠM
                await _reportService.CreateAutoReportAsync(
                    userId,
                    $"Bình luận vi phạm: {request.Content}. (Lý do AI: {aiResult.reason})",
                    "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.",
                    null,
                    request.QuestionId // Lưu lại ID của câu hỏi mà user định bình luận bậy vào
                );

                return false; // Chặn không cho lưu xuống DB
            }

            var answer = _mapper.Map<Answer>(request);
            answer.UserId = userId;
            answer.CreatedAt = DateTime.Now;

            return await _answerRepository.CreateAsync(answer);
        }

        // 🔥 THÊM HÀM UPDATE ĐỂ KIỂM TRA AI KHI SỬA BÌNH LUẬN
        public async Task<bool> UpdateAsync(AnswerUpdateRequest request, string currentUserId, bool isAdmin)
        {
            // Lấy Answer từ DB lên (Giả sử bạn có hàm GetForEditAsync hoặc GetByIdAsync trong Repo)
            var answer = await _answerRepository.GetByIdAsync(request.Id); 
            if (answer == null) return false;

            // Kiểm tra quyền: Chỉ Admin hoặc người viết bình luận mới được sửa
            if (!isAdmin && answer.UserId != currentUserId) return false;

            // BẮT BUỘC KIỂM TRA LẠI AI
            var contentToCheck = request.Content + " " + request.Content;
            var aiResult = await _aiService.CheckContentAsync(contentToCheck);

            if (aiResult.isFlagged)
            {
                await _userService.PenalizeUserAsync(currentUserId, 10, 1);

                await _reportService.CreateAutoReportAsync(
                    currentUserId,
                    $"Cố tình sửa bình luận thành nội dung vi phạm: {request.Content}. (Lý do AI: {aiResult.reason})",
                    "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.",
                    null,
                    answer.QuestionId
                );

                return false; 
            }

            _mapper.Map(request, answer); // Map dữ liệu mới đè lên record cũ
            return await _answerRepository.UpdateAsync(answer);
        }

        public async Task<bool> DeleteAsync(int answerId, string currentUserId, bool isAdmin)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null)
            {
                return false;
            }

            if (!isAdmin && answer.UserId != currentUserId)
            {
                return false; 
            }

            await _userService.AddPointsAsync(answer.UserId, -3);

            return await _answerRepository.DeleteAsync(answer);
        }

        public async Task<IEnumerable<AnswerResponse>> GetByQuestionIdAsync(int questionId)
        {
            var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
            return _mapper.Map<IEnumerable<AnswerResponse>>(answers);
        }

        public async Task<bool> DeleteByAdminAsync(int answerId)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null) 
            {
                return false;
            }

            return await _answerRepository.DeleteAsync(answer);
        }
        public async Task<IEnumerable<Answer>> GetAllForAdminAsync()
        {
            return await _answerRepository.GetAllAsync();
        }
        public async Task<IEnumerable<AnswerResponse>> GetByUserIdAsync(string userId)
        {
            var answers = await _answerRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<AnswerResponse>>(answers);
        }
        public async Task<StudyShare.DTOs.Responses.AnswerResponse> GetByIdAsync(int id)
{
    var answer = await _answerRepository.GetByIdAsync(id);
    return _mapper.Map<AnswerResponse>(answer);
}
    }
}