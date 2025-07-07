using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task UpdateCart(Cart cart);
        Task<Cart> GetCartByUserIdAsync(int userId); // Get cart details for a specific user
        //Task<bool> ClearCartAsync(string userId);
    }
}
