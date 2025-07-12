using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; } // Foreign key to the Cart table
        [ForeignKey("CartId")]
        public Cart Cart { get; set; } // Navigation property
        public int ProductId { get; set; } // Foreign key to the Product table
        public string ProductName { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } // Navigation property
        public int Quantity { get; set; }

        public int Price { get; set; } // Price per item

    }
}
