using MyWebApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.DTOs
{
    public record ProductDTO(int Id,
        [Required] string Name,
        [Required]string Description,
        [Required, DataType(DataType.Currency)]int Price,
        [Required]int Quantity,
        [Required]string Size,
        [Required]string Color,
        [Required]string Material,
        int CategoryId);
    
 
}
