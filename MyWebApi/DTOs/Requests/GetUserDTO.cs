using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record GetUserDTO(int Id,
        [Required] string Name,
        [Required] string PhoneNumber,
        [Required, EmailAddress] string Email,
        [Required] string Address,
        [Required] string City,
        [Required] string Role
        );
    
}
