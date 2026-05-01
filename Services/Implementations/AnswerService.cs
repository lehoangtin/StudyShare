using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyShare.Models;
using StudyShare.DTOs.Requests;
using StudyShare.Services.Interfaces;
using StudyShare.Repositories.Interfaces;
using StudyShare.DTOs.Responses; // Thêm dòng này
namespace StudyShare.Services.Implementations
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AnswerService(IAnswerRepository answerRepository, IUserService userService, IUserRepository userRepository, IMapper mapper)
        {
            _answerRepository = answerRepository;
            _userService = userService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(AnswerCreateRequest request, string userId)
        {
            var answer = _mapper.Map<Answer>(request);
            answer.UserId = userId;
            answer.CreatedAt = DateTime.Now;

            return await _answerRepository.CreateAsync(answer);
        }

        // Xóa các hàm DeleteByUserAsync và DeleteByAdminAsync cũ đi và thay bằng hàm này:

        public async Task<bool> DeleteAsync(int answerId, string currentUserId, bool isAdmin)
        {
            // 1. Lấy thông tin câu trả lời
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null)
            {
                return false;
            }

            // 2. Kiểm tra quyền: Chỉ Admin hoặc chính người trả lời mới được xóa
            if (!isAdmin && answer.UserId != currentUserId)
            {
                return false; // Trả về false nếu không có quyền
            }

            // 3. Xử lý nghiệp vụ: Trừ điểm người dùng (ví dụ trừ 3 điểm)
            await _userService.AddPointsAsync(answer.UserId, -3);

            // 4. Tiến hành xóa câu trả lời từ Database
            return await _answerRepository.DeleteAsync(answer);
        }
        public async Task<IEnumerable<AnswerResponse>> GetByQuestionIdAsync(int questionId)
        {
            var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
            return _mapper.Map<IEnumerable<AnswerResponse>>(answers);
        }
        public async Task<bool> DeleteByAdminAsync(int answerId)
        {
            // 1. Lấy thông tin câu trả lời
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null) 
            {
                return false;
            }

            // 2. Tiến hành xóa câu trả lời
            // (Toàn bộ logic trừ điểm user đã bị gỡ bỏ để nhường cho hàm Penalize xử lý)
            return await _answerRepository.DeleteAsync(answer);
        }
    }
}