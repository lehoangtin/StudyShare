using StudyShare.Models;
using StudyShare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace StudyShare.Repositories.Implementations
{
    public class AnswerRepository : IAnswerRepository   
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.Answers.FindAsync(id);
        }

        public async Task<Answer?> GetByIdAndUserAsync(int id, string userId)
        {
            return await _context.Answers.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<bool> CreateAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Answer answer)
        {
            // Gỡ liên kết Report (không xóa report để giữ lại lịch sử xử lý)
            var relatedReports = _context.Reports.Where(r => r.AnswerId == answer.Id).ToList();
            foreach (var r in relatedReports)
                r.AnswerId = null;

            _context.Answers.Remove(answer);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<Answer>> GetAllAsync(string? search = null)
        {
            var query = _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(a => 
                    (a.Content != null && a.Content.ToLower().Contains(lowerSearch)) ||
                    (a.User != null && a.User.FullName != null && a.User.FullName.ToLower().Contains(lowerSearch)) ||
                    (a.User != null && a.User.UserName != null && a.User.UserName.ToLower().Contains(lowerSearch))
                );
            }

            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }
        public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Answer>> GetByUserIdAsync(string userId)
        {
            return await _context.Answers
                .Include(a => a.Question) // Để hiển thị câu trả lời này thuộc câu hỏi nào
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}