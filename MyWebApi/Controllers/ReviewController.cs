using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs.Reviews;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.Security.Claims;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetProductReviews(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var reviewlist = await _reviewService.GetProductReviewsAsync(productId, page, pageSize);
            if (!reviewlist.Any())
            {
                return NotFound($"Không có đánh giá nào cho sản phẩm mã {productId}");

            }
            return Ok(reviewlist);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();

            }
            return Ok(review);
        }
        [HttpPost]
        public async Task<ActionResult<Response>> PostReview([FromBody] PostReviewDTO reviewdto, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; => String
            var response = await _reviewService.CreateReviewAsync(userId, reviewdto);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpPut]
        public async Task<ActionResult<Response>> UpdateReview(int id, [FromBody] UpdateReviewDTO req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _reviewService.UpdateReviewAsync(id, req);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }
        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteReview(int reviewid, int userid)
        {
            var response = await _reviewService.DeleteReviewAsync(reviewid, userid);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        
    }
}
