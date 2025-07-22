using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class VoucherUsage
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("VoucherId")]

        public Voucher Voucher { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
