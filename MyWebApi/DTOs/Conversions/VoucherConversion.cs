using MyWebApi.DTOs.VoucherDTOs;
using MyWebApi.Models;

namespace MyWebApi.DTOs.Conversions
{
    public static class VoucherConversion
    {
        public static Voucher ToEntity(VoucherDTO voucherDTO)
        {
            return new Voucher
            {
                Id = voucherDTO.Id,
                Code = voucherDTO.Code,
                Name = voucherDTO.Name,
                Description = voucherDTO.Description,
                Type = voucherDTO.Type,
                Value = voucherDTO.Value,
                MinOrderAmount = voucherDTO.MinOrderAmount,
                MaxDiscountAmount = voucherDTO.MaxDiscountAmount,
                UsageLimit = voucherDTO.UsageLimit,
                UsedCount = voucherDTO.UsedCount,
                StartDate = voucherDTO.StartDate,
                EndDate = voucherDTO.EndDate,
                IsActive = voucherDTO.isActive
            };

        }



        public static (VoucherDTO?, IEnumerable<VoucherDTO>?) ToDTO(Voucher? voucher, IEnumerable<Voucher>? vouchers)
        {
            if (voucher is not null || vouchers is null) {
                var singleVoucher = new VoucherDTO(
                    voucher!.Id,
                    voucher.Code,
                    voucher.Name,
                    voucher.Description,
                    voucher.Type,
                    voucher.Value,
                    voucher.MinOrderAmount,
                    voucher.MaxDiscountAmount,
                    voucher.UsageLimit,
                    voucher.UsedCount,
                    voucher.StartDate,
                    voucher.EndDate,
                    voucher.IsActive);
                return (singleVoucher, null);
            }
            if (voucher is null || vouchers is null) {
                var voucherDtos = vouchers!.Select(v => new VoucherDTO(
                    v.Id,
                    v.Code,
                    v.Name,
                    v.Description,
                    v.Type,
                    v.Value,
                    v.MinOrderAmount,
                    v.MaxDiscountAmount,
                    v.UsageLimit,
                    v.UsedCount,
                    v.StartDate,
                    v.EndDate,
                    v.IsActive)).ToList();
                return (null, voucherDtos);
            
            }
            return (null, null);

        }
    }
}
