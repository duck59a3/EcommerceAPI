using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record CartItemDTO(int Id,
       int productId,
       int cartId,
       [Required]int Price,
       [Required]int Quantity

       );
    
}
