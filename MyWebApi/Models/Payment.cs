using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerEmail { get; set; }
    }
}
