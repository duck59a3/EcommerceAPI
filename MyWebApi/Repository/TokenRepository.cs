using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;

namespace MyWebApi.Repository
{
    public class TokenRepository : Repository<RefreshToken>, ITokenRepository
    {
        private readonly ApplicationDbContext _db;
        public TokenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public Task DeleteTokenByUserAsync(int userId)
        {
            var tokens = _db.RefreshTokens.Where(t => t.UserId == userId);
            _db.RefreshTokens.RemoveRange(tokens);
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetToken(string token)
        {
            return _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
        public Task Update(RefreshToken entity)
        {
            _db.RefreshTokens.Update(entity);
            return Task.CompletedTask;
        }
    }
}
