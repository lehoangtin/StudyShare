using AutoMapper;
using StudyShare.Models;
using StudyShare.DTOs.Requests;
using StudyShare.DTOs.Responses;
using StudyShare.Services.Interfaces;
using StudyShare.Repositories.Interfaces;

namespace StudyShare.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserService _userService;
        private readonly IAIService _aiService;
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        // Đã xóa bỏ IAnswerRepository và IUserRepository vì không sử dụng tới
        public QuestionService(
            IQuestionRepository questionRepository, 
            IMapper mapper,
            IUserService userService,       
            IAIService aiService,           
            IReportService reportService)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
            _userService = userService;
            _aiService = aiService;
            _reportService = reportService;
        }

        public async Task<IEnumerable<QuestionResponse>> GetAllAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<QuestionResponse>>(questions);
        }

        public async Task<QuestionResponse?> GetByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return question == null ? null : _mapper.Map<QuestionResponse>(question);        
        }

        public async Task<QuestionUpdateRequest?> GetForEditAsync(int id)
        {
            var question = await _questionRepository.GetForEditAsync(id);
            return question == null ? null : _mapper.Map<QuestionUpdateRequest>(question);
        }

        public async Task<bool> CreateAsync(QuestionCreateRequest request, string userId)
        {
            // 1. AI KIỂM TRA NỘI DUNG
            var contentToCheck = request.Content + " " + request.Content;
            var aiResult = await _aiService.CheckContentAsync(contentToCheck);

            if (aiResult.isFlagged) 
            {
                // 2. PHẠT NGƯỜI DÙNG (Trừ 10 điểm, tăng 1 gậy)
                await _userService.PenalizeUserAsync(userId, 10, 1); 

                // 3. LƯU VÀO LỊCH SỬ VI PHẠM
                await _reportService.CreateAutoReportAsync(
                    userId, 
                    $"Nội dung vi phạm: {request.Content}. (Lý do AI: {aiResult.reason})", 
                    "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.",
                    null, 
                    null
                );
                
                return false; 
            }

            var question = _mapper.Map<Question>(request);
            question.UserId = userId; 
            question.CreatedAt = DateTime.Now;
            return await _questionRepository.CreateAsync(question);
        }

        public async Task<bool> UpdateAsync(QuestionUpdateRequest request, string currentUserId, bool isAdmin)
        {
            var question = await _questionRepository.GetForEditAsync(request.Id);
            if (question == null) return false;

            // Kiểm tra quyền: Chỉ Admin hoặc người tạo mới được sửa
            if (!isAdmin && question.UserId != currentUserId) return false;

            // 🔥 FIX BẢO MẬT: BẮT BUỘC KIỂM TRA LẠI AI KHI UPDATE
            var contentToCheck = request.Content + " " + request.Content;
            var aiResult = await _aiService.CheckContentAsync(contentToCheck);

            if (aiResult.isFlagged) 
            {
                // Phạt người dùng vì cố tình chỉnh sửa thành nội dung xấu
                await _userService.PenalizeUserAsync(currentUserId, 10, 1); 

                await _reportService.CreateAutoReportAsync(
                    currentUserId, 
                    $"Cố tình chỉnh sửa thành nội dung vi phạm: {request.Content}. (Lý do AI: {aiResult.reason})", 
                    "Hệ thống (AI) tự động phạt trừ 10 điểm và tăng 1 gậy cảnh cáo.",
                    null, 
                    question.Id
                );
                
                return false; 
            }

            _mapper.Map(request, question); // Map dữ liệu mới đè lên record cũ
            return await _questionRepository.UpdateAsync(question);
        }

        public async Task<bool> DeleteAsync(int questionId, string currentUserId, bool isAdmin)
        {
            // 1. Lấy thông tin câu hỏi
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                return false;
            }

            // 2. Kiểm tra quyền: Chỉ Admin hoặc người đặt câu hỏi mới được xóa
            if (!isAdmin && question.UserId != currentUserId)
            {
                return false; 
            }

            // 3. (Tùy chọn) Xử lý nghiệp vụ điểm số ở đây nếu có
            await _userService.AddPointsAsync(question.UserId, -5);

            // 4. Tiến hành xóa câu hỏi
            return await _questionRepository.DeleteAsync(question);
        }

        public async Task<IEnumerable<Question>> GetAllForAdminAsync()
        {
            return await _questionRepository.GetAllForAdminAsync();
        }

        public async Task<Question?> GetDetailsForAdminAsync(int id)
        {
            return await _questionRepository.GetDetailsForAdminAsync(id);
        }

        public async Task<IEnumerable<Report>> GetReportsForQuestionAsync(int questionId)
        {
            return await _questionRepository.GetReportsForQuestionAsync(questionId);
        }

        public async Task<bool> DeleteByAdminAsync(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null) 
            {
                return false;
            }
            
            return await _questionRepository.DeleteAsync(question); 
        }

        public async Task<IEnumerable<Question>> GetUserQuestionsAsync(string userId)
        {
            return await _questionRepository.GetUserQuestionsAsync(userId);
        }
    }
}