using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db) // Fix for CS7036
        {
            _db = db;
        }

        public Task UpdateAsync(ProductImage productImage) // Fix for CS1061
        {
            _db.ProductImages.Update(productImage);
            return Task.CompletedTask;
           
        }
    }
}
