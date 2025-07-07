using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record UpdateCartDTO(
        [Required]
        [Range(1,int.MaxValue)]int Quantity);
}
