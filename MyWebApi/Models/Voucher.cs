using MyWebApi.Enums;

namespace MyWebApi.Models
{
    public class Voucher
    {
        public int  Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public VoucherType Type { get; set; }
        public int Value { get; set; }
        public int? MinOrderAmount { get; set; } // Giá trị đơn hàng tối thiểu

        public int? MaxDiscountAmount { get; set; } // Giá trị giảm tối đa

        public int? UsageLimit { get; set; } // Giới hạn số lần sử dụng

        public int UsedCount { get; set; } = 0; // Số lần đã sử dụng

        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public ICollection<VoucherUse> VoucherUsages { get; set; } = new List<VoucherUse>();
    }
}
