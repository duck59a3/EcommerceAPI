using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface ITokenRepository : IRepository<RefreshToken>
    {
        Task DeleteTokenByUserAsync(int userId);
        Task<RefreshToken?> GetToken(string token);
        Task Update(RefreshToken entity);
    }
}
