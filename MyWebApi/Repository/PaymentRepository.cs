using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.Data;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class PaymentRepository : Repository<Payment>,  IPaymentRepository
    {
        //private readonly IMemoryCache _cache;
        //private readonly string _cachePrefix = "payment_";
        private readonly ApplicationDbContext _db;

        public PaymentRepository(ApplicationDbContext db) : base(db) 
        {
            //_cache = cache;
            _db = db;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            //var cacheKey = $"{_cachePrefix}{payment.Id}";
            //_cache.Set(cacheKey, payment, TimeSpan.FromHours(24));
            return await Task.FromResult(payment);

        }

        public Task UpdatePaymentAsync(Payment payment)
        {
            //var cacheKey = $"{_cachePrefix}{payment.Id}";
            //_cache.Set(cacheKey, payment, TimeSpan.FromHours(24));
            _db.Payments.Update(payment);
            return  Task.FromResult(payment);
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            
            return await _db.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            // In a real implementation, you would query the database
            return await Task.FromResult(new List<Payment>());
        }
    }
}
