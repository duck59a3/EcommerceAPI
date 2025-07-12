using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [Range(1,5, ErrorMessage = "Ratng phải từ 1 đến 5")]
        public int Rating { get; set; }
        [MaxLength(1000, ErrorMessage = "Nội dung review không được quá 1000 ký tự")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerified { get; set; } = false; // Đánh giá từ người đã mua hàng

    }
}
