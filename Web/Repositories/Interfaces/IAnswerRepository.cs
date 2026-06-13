using StudyShare.Models;
using StudyShare.DTOs.Responses;
using System.Collections.Generic;

namespace StudyShare.Repositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task<Answer?> GetByIdAsync(int id);
        Task<Answer?> GetByIdAndUserAsync(int id, string userId);
        Task<bool> CreateAsync(Answer answer);
        Task<bool> UpdateAsync(Answer answer);
        Task<bool> DeleteAsync(Answer answer);
        Task<IEnumerable<Answer>> GetAllAsync(string? search = null);
        Task<IEnumerable<Answer>> GetByQuestionIdAsync(int questionId);    
        Task<IEnumerable<Answer>> GetByUserIdAsync(string userId);
    }
}