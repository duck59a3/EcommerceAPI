using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record UpdatePaymentStatusDTO(
        [Required]int paymentId,
        string TransactionId,
        [Required]PaymentStatus paymentStatus);
}
