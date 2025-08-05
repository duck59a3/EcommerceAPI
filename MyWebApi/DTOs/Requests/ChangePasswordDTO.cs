using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record ChangePasswordDTO(
        [Required]int userId,
        [Required] string CurrentPassword,
        [Required] string NewPassword,
        [Required] string ConfirmNewPassword);
}
