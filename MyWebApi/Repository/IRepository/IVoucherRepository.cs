using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task UpdateAsync(Voucher voucher);
        Task<Voucher> GetVoucherByCodeAsync(string code);
    }
}
