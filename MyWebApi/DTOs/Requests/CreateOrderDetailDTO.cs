using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record CreateOrderDetailDTO(
        [Required]int productId,
        [Required, Range(1,int.MaxValue)]int count);

}
