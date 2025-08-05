using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record ResetPasswordDTO(
        [Required] string token,
        [Required, EmailAddress]string Email);

}
