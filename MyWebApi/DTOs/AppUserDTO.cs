using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record AppUserDTO(
        int Id,
        [Required] string Name,
        [Required]string PhoneNumber,
        [Required, EmailAddress]string Email,
        [Required] string Address,
        [Required] string City,
        [Required] string Password,
        [Required] string Role);
    
}
