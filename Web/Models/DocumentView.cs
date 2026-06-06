using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyShare.Models
{
    /// <summary>
    /// Bảng lưu trữ lịch sử xem tài liệu — mỗi cặp (UserId, DocumentId) chỉ tồn tại 1 lần
    /// để đảm bảo mỗi user chỉ được tính 1 lượt xem cho mỗi tài liệu.
    /// </summary>
    public class DocumentView
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int DocumentId { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!;

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; } = null!;
    }
}
