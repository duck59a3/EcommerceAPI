using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.VoucherDTOs
{
    public record CreateVoucherDTO(
        [Required, MaxLength(50)]string Code,
        [Required] string Name,
        [Required] string Description,
        [Required] VoucherType Type,
        [Required, Range(0.01, int.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0")]int Value,
        int? MinOrderAmount,
        int? MaxDiscountAmount,
        int? UsageLimit,
        [Required]DateTime StartDate,
        [Required]DateTime EndDate);

}
