using Microsoft.EntityFrameworkCore;
using MyWebApi.DTOs;
using MyWebApi.Models;

namespace MyWebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
                new Category { Id = 2, Name = "Books", Description = "Literature and educational materials" },
                new Category { Id = 3, Name = "Clothing", Description = "Apparel and accessories" }
            
            );
            string hash = BCrypt.Net.BCrypt.HashPassword("admin123");
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id = 2, Name = "Admin", Email = "admin@gmail.com", Password = hash, Role = "Admin", Address ="abc",PhoneNumber="03313", City="aaa" }
                );

            //        modelBuilder.Entity<VoucherUse>()
            //   .HasOne(v => v.User)
            //.WithMany(u => u.VoucherUsages)
            //.HasForeignKey(v => v.UserId)
            //.OnDelete(DeleteBehavior.Restrict)// hoặc NoAction
            //        .OnDelete(DeleteBehavior.NoAction);

            //        modelBuilder.Entity<VoucherUse>()
            //            .HasOne(v => v.Voucher)
            //            .WithMany(vc => vc.VoucherUsages)
            //            .HasForeignKey(v => v.VoucherId)
            //            .OnDelete(DeleteBehavior.Restrict);

            //        modelBuilder.Entity<VoucherUse>()
            //            .HasOne(v => v.Order)
            //            .WithMany(o => o.VoucherUsages)
            //            .HasForeignKey(v => v.OrderId)
            //            .OnDelete(DeleteBehavior.Restrict)
            //            .OnDelete(DeleteBehavior.NoAction);







        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVideo> ProductVideos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
