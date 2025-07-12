using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record CreateOrderDetailDTO(
        [Required]int productId,
        [Required]string productName,
        [Required, Range(1,int.MaxValue)]int count);

}