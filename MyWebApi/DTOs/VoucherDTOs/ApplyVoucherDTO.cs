using System.ComponentModel.DataAnnotations;

namespace MyWebApi.DTOs.VoucherDTOs
{
    public record ApplyVoucherDTO(
        [Required]string Code,
        [Required]int OrderTotal); //tổng tiền hiện tại

}
