using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record UpdateOrderStatusDTO(
        [Required] OrderStatus orderStatus,
        [Required] PaymentStatus paymentStatus,
        string notes);

}
