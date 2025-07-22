using MyWebApi.DTOs.VoucherDTOs;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IVoucherService : IGenericService<Voucher>
    {
        Task<Response> CreateVoucherAsync(CreateVoucherDTO createVoucherDto);
        Task<Response> UpdateVoucherAsync(int voucherId, CreateVoucherDTO updateVoucherDto);
        Task<Response> DeleteVoucherAsync(int voucherId);
        Task<VoucherDTO> GetVoucherByIdAsync(int voucherId);
        Task<VoucherDTO> GetVoucherByCodeAsync(string code);
        Task<IEnumerable<VoucherDTO>> GetActiveVouchersAsync();
        Task<VoucherDiscountResponse> ValidateAndCalculateDiscountAsync(int userId, ApplyVoucherDTO applyVoucherDto);
        Task<Response> ApplyVoucherAsync(int userId, int voucherId, int orderId, int discountAmount);
        Task<bool> CanUserUseVoucherAsync(int userId, int voucherId);
    }
}
