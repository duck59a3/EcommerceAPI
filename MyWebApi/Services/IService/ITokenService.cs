using MyWebApi.DTOs;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface ITokenService : IGenericService<RefreshToken>
    {
        Task<Response> RenewToken(TokenResponseDTO dto);
        Task<TokenResponseDTO> GenerateToken(AppUser user);
    }
    
}
