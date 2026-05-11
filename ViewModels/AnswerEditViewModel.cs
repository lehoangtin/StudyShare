using System.ComponentModel.DataAnnotations;

namespace StudyShare.ViewModels
{
    public class AnswerEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; } // Để quay về trang chi tiết câu hỏi sau khi sửa

        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        public string Content { get; set; }
    }
}