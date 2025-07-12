using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Reviews
{
    public record ReviewDTO(
        int Id,
        [Required]int ProductId,
        [Required]int UserId,
        [Required]string Username,
        [Required]int Rating,
        [Required]string Content,
        DateTime CreatedAt

        );

}
