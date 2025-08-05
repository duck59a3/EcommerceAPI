using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApi.DTOs;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyWebApi.Services
{
    public class TokenService : GenericService<RefreshToken>, ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;
        public TokenService(IUnitOfWork unitOfWork,ITokenRepository tokenRepository, IConfiguration configuration) : base(unitOfWork, tokenRepository)
        {
            _unitOfWork = unitOfWork;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }
     

        public async Task<Response> RenewToken(TokenResponseDTO dto)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Key").Value!);
            var param = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero, // Không cho phép trễ thời gian
                ValidateLifetime = false,
            };
            try
            {
                //check access token
                var tokenInVerification = jwtTokenHandler.ValidateToken(dto.AccessToken, param, out var validatedToken);
                //check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return new Response(false, "Token không hợp lệ");
                    }
                }
                //check expiration

                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value!);
                var expiryDateTime = ConverUnixTimeToDateTime(utcExpiryDate);

                if (expiryDateTime > DateTime.UtcNow)
                {
                    return new Response(false, "Token chưa hết hạn");
                }
                //check refresh token
                var existrefreshToken = await _unitOfWork.RefreshTokens.GetToken(dto.RefreshToken);
                if (existrefreshToken == null)
                {
                    return new Response(false, "Refresh token không tồn tại");
                }
                //check refresh token is used or revoked
                if (existrefreshToken.IsUsed)
                {
                    return new Response(false, "Refresh token đã được sử dụng ");
                }
                if (existrefreshToken.IsRevoked)
                {
                    return new Response(false, "Refresh token đã bị thu hồi");
                }
                //check access token == token in refresh token
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (jti != existrefreshToken.JwtID)
                {
                    return new Response(false, "Access token không khớp");
                }
                //update refresh token
                existrefreshToken.IsUsed = true;
                existrefreshToken.IsRevoked = true;
                await _unitOfWork.RefreshTokens.Update(existrefreshToken);
                await _unitOfWork.SaveAsync();
                //generate new token
                var user = await _unitOfWork.Users.GetUserByIdAsync(existrefreshToken.UserId);
                var token = await GenerateToken(user);
                return new Response(true, "Tạo mới token thành công");
               

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Token không hợp lệ hoặc đã hết hạn");
            }

        }

        private DateTime ConverUnixTimeToDateTime(long utcExpiryDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpiryDate).ToUniversalTime();
            return dateTimeInterval;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<TokenResponseDTO> GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);// Sử dụng HMAC SHA256 để ký token
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Name),
                new (ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Role, user.Role!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenentity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 ngày
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                JwtID = token.Id

            };
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenentity);
            await _unitOfWork.SaveAsync();



            return new TokenResponseDTO(accessToken, refreshToken);


        }
    }
}
