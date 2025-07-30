using MyWebApi.DTOs.Reviews;
using MyWebApi.Helpers.QueryParameters;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IReviewService : IGenericService<Review>
    {
        Task<Response> CreateReviewAsync(int userId, PostReviewDTO req);
        Task<Response> UpdateReviewAsync(int reviewId, UpdateReviewDTO updateReviewDto);
        Task<Response> DeleteReviewAsync(int reviewId, int userId);
        Task<ReviewDTO> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<ReviewDTO>> GetProductReviewsAsync(int productId,int page = 1, int pageSize = 5);
        Task<IEnumerable<ReviewDTO>> GetUserReviewsAsync(int userId, ReviewQuery query);
        //Task<ProductReviewSummary> GetProductReviewSummaryAsync(int productId);
        Task<bool> CanUserReviewProductAsync(int userId, int productId);
        Task<Response> UploadReviewImage(int reviewId, IFormFile file);
    }
}
