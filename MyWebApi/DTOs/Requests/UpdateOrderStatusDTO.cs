using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.Requests
{
    public record UpdateOrderStatusDTO(
        [Required] OrderStatus orderStatus);

}
