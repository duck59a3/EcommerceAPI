using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // Tên sản phẩm
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
        public int Price { get; set; } // Giá sản phẩm tại thời điểm đặt hàng
    }
}
