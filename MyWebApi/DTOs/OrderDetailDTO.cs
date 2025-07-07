using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record OrderDetailDTO(
        int Id,
        [Required]int productId,
        [Required] string productName,
        [Required] int orderId,
        [Required] int count,
        [Required] int price
        ); 
}
