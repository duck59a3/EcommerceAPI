using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        private readonly ApplicationDbContext _db;
        public VoucherRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _db.Vouchers.FirstOrDefaultAsync(v => v.Code.ToLower() == code.ToLower());
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            _db.Vouchers.Update(voucher);
  
        }
    }
}
