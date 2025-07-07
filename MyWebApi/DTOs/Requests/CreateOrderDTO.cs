using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record CreateOrderDTO(
        [Required]int userId,
        string name,
        string phoneNumber,
        string address,
        string city,
        [Required]string paymentMethod,
        List<CreateOrderDetailDTO> items ) ;
}
