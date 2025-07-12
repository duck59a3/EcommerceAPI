using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db) {
        
            _db = db;
        }

        public async Task<Review> GetReviewById(int id)
        {
            return await _db.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Review> GetReviewsByUserIdAndProductId(int userId, int productId)
        {
            return await _db.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public Task Update(Review review)
        {
            _db.Reviews.Update(review);
            return Task.CompletedTask;
        }
    }
}
