using MyWebApi.Enums;
using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
        Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
    }
}
