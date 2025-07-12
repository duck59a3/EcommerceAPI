using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record CODPaymentUpdateDTO(
        [Required]int orderId,
        [Required]int paymentId);
}
