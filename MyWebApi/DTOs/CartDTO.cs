using MyWebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record CartDTO(int Id,
        [Required] List<CartItemDTO> CartItemsDTO,
        [Required] int TotalPrice,
        DateTime LastUpdated);
    
   
}
