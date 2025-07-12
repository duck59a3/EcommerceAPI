using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record PaymentDTO(int Id,
        [Required]int orderId,
        [Required]PaymentMethod paymentMethod,
        [Required]string transactionId,
        [Required]int amount,
        [Required]PaymentStatus PaymentStatus);

}
