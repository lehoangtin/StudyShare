using AutoMapper;
using StudyShare.DTOs.Responses;
using StudyShare.Repositories.Interfaces;
using StudyShare.Services.Interfaces;
using StudyShare.Models;
namespace StudyShare.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }
        public async Task<Report> GetByIdAsync(int reportId)
        {
            return await _reportRepository.GetByIdAsync(reportId);
        }

        public async Task<IEnumerable<ReportResponse>> GetReportsForUserAsync(string userId)
        {
            var reports = await _reportRepository.GetReportsByTargetUserAsync(userId);
            return _mapper.Map<IEnumerable<ReportResponse>>(reports);
        }

        public async Task<bool> ResolveReportAsync(int reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;
            report.IsResolved = true; 
            await _reportRepository.UpdateAsync(report);
            return true;
        }
        public async Task<bool> ResolveWithActionAsync(int reportId, string action)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            report.IsResolved = true;
            report.ActionTaken = action; // Lưu lại lý do hoặc hình thức phạt

            await _reportRepository.UpdateAsync(report);
            return true;
        }
        public async Task<IEnumerable<ReportResponse>> GetAllPendingReportsAsync()
        {
            var reports = await _reportRepository.GetAllPendingReportsAsync();
            return _mapper.Map<IEnumerable<ReportResponse>>(reports);
        }
        public async Task<IEnumerable<ReportResponse>> GetResolvedReportsAsync()
        {
            var reports = await _reportRepository.GetAllResolvedReportsAsync();
            return _mapper.Map<IEnumerable<ReportResponse>>(reports);
        }
        // Thêm hàm này vào class ReportService
// Trong ReportService.cs
        public async Task<int> CreateAutoReportAsync(string targetUserId, string reason, string actionTaken, int? docId = null, int? qid = null, string? targetContentSnapshot = null)
        {
            var report = new Report
            {
                TargetUserId = targetUserId,
                ReporterUserId = null, 
                Reason = "[AI Phát hiện] " + reason,
                DocumentId = docId,
                QuestionId = qid,
                IsResolved = true, 
                ActionTaken = actionTaken,
                TargetContentSnapshot = targetContentSnapshot,
                CreatedAt = DateTime.Now
            };

            var savedReport = await _reportRepository.CreateAsync(report); 
            return savedReport.Id;
        }
        public async Task<bool> CreateReportAsync(string reporterId, string targetUserId, string reason, int? questionId = null, int? answerId = null, string? targetContentSnapshot = null)
        {
            var report = new Report
            {
                ReporterUserId = reporterId,
                TargetUserId = targetUserId,
                Reason = reason,
                QuestionId = questionId,
                AnswerId = answerId,
                IsResolved = false,
                TargetContentSnapshot = targetContentSnapshot,
                CreatedAt = DateTime.Now
            };

            await _reportRepository.CreateAsync(report);
            return true;
        }
    }
}
