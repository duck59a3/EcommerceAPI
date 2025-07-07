using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            return await _db.Carts.Include(c => c.CartItems)
                                  .ThenInclude(ci => ci.Product)
                                  .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public Task UpdateCart(Cart cart)
        {
            _db.Carts.AddAsync(cart);
            return Task.CompletedTask;
        }
    }

}
