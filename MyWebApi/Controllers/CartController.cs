using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.Security.Claims;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDTO>> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound(new { Message = "Cart not found for the specified user." });
            }
            return Ok(cart);
        }
        [HttpPost("{userId}/items")]
        public async Task<ActionResult<Response>> AddToCart(int userId,[FromBody] AddToCartDTO request) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _cartService.AddToCartAsync(userId, request);
            return response.Flag ? Ok(response) : BadRequest(response);

        }
        [HttpPut("{userId}/items/{cartItemId}")]
        public async Task<ActionResult<Response>> UpdateCart(int userId, int cartItemId, [FromBody] UpdateCartDTO updateCartDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _cartService.UpdateCartAsync(userId, cartItemId, updateCartDTO);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
        [HttpDelete("{userId}/items/{cartItemId}")]
        public async Task<ActionResult<Response>> RemoveFromCart(int userId, int cartItemId)
        {
            var response = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        // GetTotal, ApplyVoucher, ClearCart, etc. 
    }
}
