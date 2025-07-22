using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class VoucherUsageRepository : Repository<VoucherUsage>, IVoucherUsageRepository
    {
        private readonly ApplicationDbContext _db;
        public VoucherUsageRepository(ApplicationDbContext db) : base(db) {
            _db = db;
        }
    }
}
