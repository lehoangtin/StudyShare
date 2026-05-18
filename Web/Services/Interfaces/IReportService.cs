using StudyShare.DTOs.Responses;
using StudyShare.Models;
namespace StudyShare.Services.Interfaces
{
    public interface IReportService
    {
        Task<Report> GetByIdAsync(int reportId);
        Task<IEnumerable<ReportResponse>> GetReportsForUserAsync(string userId);
        Task<bool> ResolveReportAsync(int reportId); // Xử lý báo cáo xong, nhưng chưa ghi lại hành động đã thực hiện
        Task<bool> ResolveWithActionAsync(int reportId, string action); // Xử lý báo cáo xong và ghi lại hành động đã thực hiện
        Task<IEnumerable<ReportResponse>> GetAllPendingReportsAsync(); // Lấy tất cả báo cáo chưa được xử lý
        Task<IEnumerable<ReportResponse>> GetResolvedReportsAsync();
        Task<bool> CreateReportAsync(string reporterId, string targetUserId, string reason, int? questionId = null, int? answerId = null);
        Task<int> CreateAutoReportAsync(string targetUserId, string reason, string actionTaken, int? docId = null, int? qid = null);
    }
}
