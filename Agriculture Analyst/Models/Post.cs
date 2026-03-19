using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agriculture_Analyst.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Content { get; set; } // Nội dung status

        public string? ImageUrl { get; set; } // Ảnh đính kèm (có thể null)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Tùy chọn: Gắn thẻ (Đính kèm) một vụ mùa để công bố báo cáo
        public int? PlantId { get; set; }
        [ForeignKey("PlantId")]
        public Plant? Plant { get; set; }
    }
}