using MyWebApi.Enums;
using MyWebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs
{
    public record OrderDTO(
        int Id,
        [Required] int UserId,
        DateTime OrderDate,
        [Required] OrderStatus status,
        [Required] int TotalPrice,
        int ShippingCost,
        [Required] string PaymentMethod,
        [Required] PaymentStatus PaymentStatus,
        string Notes,
        DateTime CreatedAt,
        [Required] string FullName,
        [Required] string Phone,
        [Required] string Address,
        [Required] string City,
        [Required] List<OrderDetailDTO> OrderDetails);

}
