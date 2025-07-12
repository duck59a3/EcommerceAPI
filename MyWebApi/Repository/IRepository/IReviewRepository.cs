using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review> GetReviewsByUserIdAndProductId(int userId, int productId);
        Task Update(Review review); 
        Task<Review> GetReviewById(int id);
    }
}
