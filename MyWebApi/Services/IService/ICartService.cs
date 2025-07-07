using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface ICartService :IGenericService<Cart>
    {
        Task<CartDTO> GetCartByUserIdAsync(int userId); // cart details for a specific user
        Task<Response> AddToCartAsync(int userId, AddToCartDTO request); // add a new item to the cart
        Task<Response> RemoveFromCartAsync(int userId, int cartItemId); // remove an item from the cart
        Task<Response> UpdateCartAsync(int userId, int cartItemId, UpdateCartDTO updateCartDTO);
        Task<Response> ClearCartAsync(int userId);
        Task<decimal> CalculateTotalPriceAsync(int userId);
        //Apply voucher
    }
}
