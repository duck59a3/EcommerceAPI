using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Reviews
{
    public record PostReviewDTO(
        [Required]int ProductId,
        [Required, Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]int Rating,
        [Required, MaxLength(1000, ErrorMessage = "Nội dung review không được quá 1000 ký tự")]string Content);
}
