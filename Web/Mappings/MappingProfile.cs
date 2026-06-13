using AutoMapper;
using StudyShare.Models;
using StudyShare.DTOs.Requests;
using StudyShare.DTOs.Responses;
using System.Linq;
using System.Collections.Generic;
using StudyShare.ViewModels;

namespace StudyShare.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. APP USER MAPPINGS
            CreateMap<AppUser, UserResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.WarningCount, opt => opt.MapFrom(src => src.WarningCount))
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.IsBanned));

            CreateMap<AppUser, UserViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Documents != null ? src.Documents.Count : 0))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions != null ? src.Questions.Count : 0))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.IsBanned))
                .ForMember(dest => dest.WarningCount, opt => opt.MapFrom(src => src.WarningCount))
                .ForMember(dest => dest.Role, opt => opt.Ignore()); // Bảo AutoMapper bỏ qua Role, mình sẽ tự gán sau

            CreateMap<UserResponse, UserViewModel>()
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.IsBanned))
                .ForMember(dest => dest.WarningCount, opt => opt.MapFrom(src => src.WarningCount));

            CreateMap<AppUser, UserEditViewModel>();
            CreateMap<UserEditViewModel, ProfileUpdateRequest>().ReverseMap();
            // Map từ Entity (AppUser) sang ViewModel để hiển thị
            // 2. DOCUMENT MAPPINGS
            CreateMap<DocumentCreateRequest, Document>();
            CreateMap<DocumentCreateViewModel, DocumentCreateRequest>();
            CreateMap<DocumentEditViewModel, DocumentUpdateRequest>().ReverseMap();

            CreateMap<Document, DocumentResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Chưa phân loại"))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "")); // THÊM DÒNG NÀY
            CreateMap<DocumentResponse, DocumentViewModel>();

            CreateMap<Document, DocumentViewModel>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Chưa phân loại"))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "")); // THÊM DÒNG NÀY
            // 3. QUESTION MAPPINGS
            CreateMap<QuestionCreateRequest, Question>();
            CreateMap<QuestionCreateViewModel, QuestionCreateRequest>();
            CreateMap<QuestionEditViewModel, QuestionUpdateRequest>().ReverseMap();
            CreateMap<QuestionUpdateRequest, Question>();
            CreateMap<Question, QuestionResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers != null ? src.Answers.Count : 0))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "")) // THÊM DÒNG NÀY             
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User != null ? src.User.Avatar : null));
            CreateMap<QuestionResponse, QuestionViewModel>();

            CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers != null ? src.Answers.Count : 0))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "")) // THÊM DÒNG NÀY            // 4. ANSWER MAPPINGS
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User != null ? src.User.Avatar : null));
            CreateMap<AnswerCreateRequest, Answer>();
            CreateMap<AnswerCreateViewModel, AnswerCreateRequest>();
            CreateMap<AnswerCreateViewModel, AnswerCreateRequest>();
            CreateMap<AnswerEditViewModel, AnswerUpdateRequest>();
            CreateMap<Answer, AnswerResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User != null ? src.User.Avatar : null));
            
            CreateMap<AnswerResponse, AnswerViewModel>();

            CreateMap<Answer, AnswerViewModel>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Ẩn danh"))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.MapFrom(src => src.User != null ? src.User.Avatar : null));

            // 5. CATEGORY MAPPINGS
            // CREATE
            CreateMap<CategoryCreateViewModel, CategoryCreateRequest>();
            CreateMap<CategoryCreateRequest, Category>();

            // UPDATE
            CreateMap<CategoryEditViewModel, CategoryUpdateRequest>().ReverseMap(); // Thêm ReverseMap ở đây
            CreateMap<CategoryUpdateRequest, Category>().ReverseMap(); // Thêm ReverseMap ở đây

            // RESPONSE
            CreateMap<Category, CategoryResponse>();

            // VIEWMODEL
            CreateMap<CategoryResponse, CategoryViewModel>();
            CreateMap<CategoryResponse, CategoryEditViewModel>();
            // 6. REPORT MAPPINGS
            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => 
                    src.Reporter != null ? src.Reporter.FullName : "Hệ thống (AI)"))
                .ForMember(dest => dest.TargetUserName, opt => opt.MapFrom(src => 
                    src.Target != null ? src.Target.FullName : "N/A"))
                .ForMember(dest => dest.TargetUserId, opt => opt.MapFrom(src => 
                    src.Target != null ? src.Target.Id : null))
                .ForMember(dest => dest.TargetContent, opt => opt.MapFrom(src => 
                    src.TargetContentSnapshot ?? (
                    src.Answer != null ? src.Answer.Content : 
                    src.Question != null ? src.Question.Content : 
                    src.Document != null ? src.Document.Title : null)));

            CreateMap<ReportResponse, ReportViewModel>();

            CreateMap<Report, ReportViewModel>()
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter != null ? src.Reporter.FullName : "Hệ thống (AI)"))
                .ForMember(dest => dest.TargetUserName, opt => opt.MapFrom(src => src.Target != null ? src.Target.FullName : "N/A"));
        }
    }
}