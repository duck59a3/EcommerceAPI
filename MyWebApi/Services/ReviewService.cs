using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs.Reviews;
using MyWebApi.Enums;
using MyWebApi.Helpers.QueryParameters;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyWebApi.Services
{
    public class ReviewService : GenericService<Review>, IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;
        private readonly ApplicationDbContext _db;
        public ReviewService(IUnitOfWork unitOfWork, IReviewRepository repository,ApplicationDbContext db) : base(unitOfWork,repository)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = repository;
            _db = db;
        }
        public async Task<bool> CanUserReviewProductAsync(int userId, int productId)
        {
            var hasPurchased = await _db.OrderDetails
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.ProductId == productId &&
                              oi.Order.UserId == userId &&
                              oi.Order.Status == OrderStatus.Delivered);

            return hasPurchased;
        }

        public async Task<Response> CreateReviewAsync(int userId, PostReviewDTO req)
        {
            try
            {
                var existingreviews = await _reviewRepo.GetReviewsByUserIdAndProductId(userId, req.ProductId);
                if (existingreviews != null) {
                    throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi");
                }
                // Kiểm tra xem user có quyền review không(đã mua sản phẩm)
                var canReview = await CanUserReviewProductAsync(userId, req.ProductId);
                var review = new Review
                    {
                        ProductId = req.ProductId,
                        UserId = userId,
                        Rating = req.Rating,
                        Content = req.Content,
                        CreatedAt = DateTime.UtcNow,
                        IsVerified = canReview,

                    };
                    await _unitOfWork.Reviews.AddAsync(review);
                    await _unitOfWork.SaveAsync();
                
                return new Response(true, $"Đánh giá sản phẩm mã {req.ProductId} thành công");
                

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Đánh giá bị lỗi");
            }
        }

        public async Task<Response> DeleteReviewAsync(int reviewId, int userId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    throw new InvalidOperationException("Không tồn tại đánh giá này");

                }
                await _unitOfWork.Reviews.RemoveAsync(review);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Xóa đánh giá thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Lỗi xóa đánh giá");
            }

        }

        public async Task<IEnumerable<ReviewDTO>> GetProductReviewsAsync(int productId, int page = 1, int pageSize = 5)
        {
            var productreviews = await _db.Reviews.Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewDTO(r.Id,
                r.ProductId,
                r.UserId,
                r.User.Name ?? "Unknown",
                r.Rating,
                r.Content,
                r.CreatedAt
                )).ToListAsync();


            return  productreviews;
            
           
            
               
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetReviewById(reviewId);
            if (review == null)
            {
                return null;
            }
            var getreview = new ReviewDTO(review.Id,
                review.ProductId,
                review.UserId,
                review.User?.Name ?? "Unknown",
                review.Rating,
                review.Content,
                review.CreatedAt);
            return getreview;


        }

        public Task<IEnumerable<ReviewDTO>> GetUserReviewsAsync(int userId, ReviewQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> UpdateReviewAsync(int reviewId, UpdateReviewDTO updateReviewDto)
        {
            try
            {
                var existingreview = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (existingreview == null)
                {
                    throw new InvalidOperationException("Sản phẩm chưa được bạn đánh giá");
                }
                else
                {
                    existingreview.Rating = updateReviewDto.Rating;
                    existingreview.Content = updateReviewDto.Content;
                    existingreview.UpdatedAt = DateTime.Now;
                }
                await _reviewRepo.Update(existingreview);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Cập nhật đánh giá thành công");
            }
            catch(Exception ex) 
            {
                    LogException.LogExceptions(ex);
                    return new Response(false, "Cập nhật đánh giá thất bại");
            }
        }
    }
}
