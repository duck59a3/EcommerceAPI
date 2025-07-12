using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record CreateOrderDTO(
        [Required]int userId,
        [Required]string name,
        [Required] string phoneNumber,
        [Required] string address,
        [Required] string city,
        string notes,
        [Required]string paymentMethod,
        List<CreateOrderDetailDTO> items ) ;
}
