using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record PaymentRequestDTO(
       [Required]int orderId,
       [Required,Range(1, int.MaxValue)]int amount,
       [Required]PaymentMethod paymentMethod,
       string customerEmail);

}
