using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class VoucherUse
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int DiscountAmount { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        

        public Voucher Voucher { get; set; }
        
        public AppUser User { get; set; }
        
        public Order Order { get; set; }
    }
}
