using System.ComponentModel.DataAnnotations;

namespace StudyShare.DTOs.Requests
{
    public class AnswerUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        public string Content { get; set; }
    }
}