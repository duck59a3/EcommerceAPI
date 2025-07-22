using MyWebApi.Enums;

namespace MyWebApi.DTOs.VoucherDTOs
{
    public record VoucherDTO(
        int Id,
        string Code,
        string Name,
        string Description,
        VoucherType Type,
        int Value,
        int? MinOrderAmount,
        int? MaxDiscountAmount,
        int? UsageLimit,
        int UsedCount,
        DateTime StartDate,
        DateTime EndDate,
        bool isActive);

}
