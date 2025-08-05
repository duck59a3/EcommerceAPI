using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs.Conversions;
using MyWebApi.DTOs.VoucherDTOs;
using MyWebApi.Enums;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace MyWebApi.Services
{
    public class VoucherService : GenericService<Voucher>, IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public VoucherService(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork, ApplicationDbContext db) : base(unitOfWork, voucherRepository)
        {
            _voucherRepository = voucherRepository;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        //public async Task<bool> CanUserUseVoucherAsync(int userId, int voucherId)
        //{
        //    // Kiểm tra xem user đã sử dụng voucher này chưa
        //    var hasUsed = await _db.VouchersUses
        //        .AnyAsync(vu => vu.UserId == userId && vu.VoucherId == voucherId);

        //    return !hasUsed;
        //}

        public async Task<Response> CreateVoucherAsync(CreateVoucherDTO createVoucherDto)
        {
            try
            {
                var existVoucher = await _db.Vouchers.FirstOrDefaultAsync(v =>
                 v.Code.ToLower() == createVoucherDto.Code.ToLower());
                if (existVoucher != null)
                {
                    return new Response(false, "Mã code đã tồn tại");
                }
                var voucher = new Voucher
                {
                    Code = createVoucherDto.Code.ToUpper(),
                    Name = createVoucherDto.Name,
                    Description = createVoucherDto.Description,
                    Type = createVoucherDto.Type,
                    Value = createVoucherDto.Value,
                    MinOrderAmount = createVoucherDto.MinOrderAmount,
                    MaxDiscountAmount = createVoucherDto.MaxDiscountAmount,
                    UsageLimit = createVoucherDto.UsageLimit,
                    StartDate = createVoucherDto.StartDate,
                    EndDate = createVoucherDto.EndDate
                };
                await _unitOfWork.Vouchers.AddAsync(voucher);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Thêm voucher thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Có lỗi xảy ra khi thêm voucher");
            }
        }

        public async Task<Response> DeleteVoucherAsync(int voucherId)
        {
            var existVoucher = await _voucherRepository.GetByIdAsync(voucherId);
            if (existVoucher == null)
            {
                return new Response(false, "Voucher không tồn tại");
            }
            if (existVoucher.UsedCount > 0)
            {
                throw new InvalidOperationException("Không thể xóa voucher đã được sử dụng");
            }
            await _unitOfWork.Vouchers.RemoveAsync(existVoucher);
            await _unitOfWork.SaveAsync();
            return new Response(true, "Xóa voucher thành công");
        }

        public async Task<IEnumerable<VoucherDTO>> GetActiveVouchersAsync()
        {
            try
            {
                var now = DateTime.Now;
                var vouchers = await _db.Vouchers
                    .Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now)
                    .OrderByDescending(v => v.CreatedAt)
                    .ToListAsync();
                var (_, list) = VoucherConversion.ToDTO(null, vouchers);
                return list;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Lỗi khi lấy ra voucher.", ex);
            }
        }

        public async Task<VoucherDTO> GetVoucherByCodeAsync(string code)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers.GetVoucherByCodeAsync(code);
                var (single, _) = VoucherConversion.ToDTO(voucher, null);
                return single;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Lỗi khi lấy ra voucher.", ex);
            }
        }

        public async Task<VoucherDTO> GetVoucherByIdAsync(int voucherId)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers.GetByIdAsync(voucherId);
                var (single, _) = VoucherConversion.ToDTO(voucher, null);
                return single;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Lỗi khi lấy ra voucher.", ex);
            }
        }

        public async Task<Response> UpdateVoucherAsync(int voucherId, CreateVoucherDTO updateVoucherDto)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(voucherId);
                if (voucher == null)
                {
                    return new Response(false, "Không tìm thấy voucher");
                }
                if (voucher.Code.ToLower() != updateVoucherDto.Code.ToLower())
                {
                    var existingVoucher = await _db.Vouchers
                        .FirstOrDefaultAsync(v => v.Code.ToLower() == updateVoucherDto.Code.ToLower() && v.Id != voucherId);

                    if (existingVoucher != null)
                    {
                        throw new InvalidOperationException("Mã voucher đã tồn tại");
                    }
                }
                voucher.Code = updateVoucherDto.Code.ToUpper();
                voucher.Name = updateVoucherDto.Name;
                voucher.Description = updateVoucherDto.Description;
                voucher.Type = updateVoucherDto.Type;
                voucher.Value = updateVoucherDto.Value;
                voucher.MinOrderAmount = updateVoucherDto.MinOrderAmount;
                voucher.MaxDiscountAmount = updateVoucherDto.MaxDiscountAmount;
                voucher.UsageLimit = updateVoucherDto.UsageLimit;
                voucher.StartDate = updateVoucherDto.StartDate;
                voucher.EndDate = updateVoucherDto.EndDate;

                await _unitOfWork.Vouchers.UpdateAsync(voucher);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Cập nhật voucher thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Cập nhật voucher thất bại");
            }
        }

        public async Task<Response> ApplyVoucherAsync(int userId, int voucherId, int orderId, int discountAmount)
        {
            try
            {
                var voucher = await _unitOfWork.Vouchers.GetByIdAsync(voucherId);
                if (voucher == null)
                {
                    return new Response(false, "Voucher không tồn tại");
                }
                var voucherUsage = new VoucherUse
                {
                    VoucherId = voucherId,
                    UserId = userId,
                    OrderId = orderId,
                    DiscountAmount = discountAmount
                };
                await _unitOfWork.VouchersUsage.AddAsync(voucherUsage);
                voucher.UsedCount++;
                await _unitOfWork.SaveAsync();
                return new Response(true, "Áp mã khuyến mãi thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Áp dụng thất bại");
            }
        }

        public async Task<VoucherDiscountResponse> ValidateAndCalculateDiscountAsync(int userId, ApplyVoucherDTO applyVoucherDto)
        {
            var voucher = await _unitOfWork.Vouchers.GetVoucherByCodeAsync(applyVoucherDto.Code.ToLower());
            if (voucher == null)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = "Mã voucher không tồn tại"
                };
            }
            if (!voucher.IsActive)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = "Voucher đã bị vô hiệu hóa"
                };
            }
            var now = DateTime.UtcNow;

            // Kiểm tra thời gian hiệu lực
            if (now < voucher.StartDate)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = "Voucher chưa có hiệu lực"
                };
            }
            if (now > voucher.EndDate)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = "Voucher đã hết hạn"
                };
            }
            if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit.Value)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = "Voucher đã hết lượt sử dụng"
                };
            }
            if (voucher.MinOrderAmount.HasValue && applyVoucherDto.OrderTotal < voucher.MinOrderAmount.Value)
            {
                return new VoucherDiscountResponse
                {
                    IsValid = false,
                    Message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount.Value:C} để sử dụng voucher này"
                };
            }
            // Kiểm tra user có thể sử dụng voucher không
            //var canUse = await CanUserUseVoucherAsync(userId, voucher.Id);
            //if (!canUse)
            //{
            //    return new VoucherDiscountResponse
            //    {
            //        IsValid = false,
            //        Message = "Bạn đã sử dụng voucher này rồi"
            //    };
            //}
            int discountAmount = 0;

            if (voucher.Type == VoucherType.Percentage)
            {
                discountAmount = applyVoucherDto.OrderTotal * (voucher.Value / 100);

                // Áp dụng giới hạn giảm giá tối đa
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                {
                    discountAmount = voucher.MaxDiscountAmount.Value;
                }
            }
            else if (voucher.Type == VoucherType.FixedAmount)
            {
                discountAmount = Math.Min(voucher.Value, applyVoucherDto.OrderTotal);
            }

            var finalAmount = applyVoucherDto.OrderTotal - discountAmount;
            var (getVoucher, _) = VoucherConversion.ToDTO(voucher, null);

            return new VoucherDiscountResponse
            {
                IsValid = true,
                Message = "Voucher hợp lệ",
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                Voucher = getVoucher!
            };

        }

        public Task<bool> CanUserUseVoucherAsync(int userId, int voucherId)
        {
            throw new NotImplementedException();
        }
    }
}
