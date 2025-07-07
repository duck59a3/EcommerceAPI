using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key to the User table
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; } // Navigation property to the User table
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TotalPrice { get; set; } 
    }
}
