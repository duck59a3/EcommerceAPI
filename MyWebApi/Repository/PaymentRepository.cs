using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.Data;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        //private readonly IMemoryCache _cache;
        //private readonly string _cachePrefix = "payment_";
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            //_cache = cache;
            _context = context;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            //var cacheKey = $"{_cachePrefix}{payment.Id}";
            //_cache.Set(cacheKey, payment, TimeSpan.FromHours(24));
            return await Task.FromResult(payment);

        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            //var cacheKey = $"{_cachePrefix}{payment.Id}";
            //_cache.Set(cacheKey, payment, TimeSpan.FromHours(24));
            return await Task.FromResult(payment);
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            
            return await _context.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            // In a real implementation, you would query the database
            return await Task.FromResult(new List<Payment>());
        }
    }
}
