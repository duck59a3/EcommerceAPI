using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyWebApi.Services
{
    // Fix for CS1722: Base class 'GenericService<AppUser>' must come before any interfaces
    public class UserService :  IAppUser
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        public UserService(IUnitOfWork unitOfWork, IUserRepository repository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<GetUserDTO>> GetAllUsers()
        {
            try
            {
                var list = await _unitOfWork.Users.GetAllUsersAsync();
                return list.Select(user => new GetUserDTO(
                    user.Id,
                    user.Name!,
                    user.PhoneNumber!,
                    user.Email!,
                    user.Address!,
                    user.City!,
                    user.Role!
                )).ToList();


            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Lỗi khi lấy tất cả đối tượng");
            }
        }

        public async Task<GetUserDTO> GetUserById(int id)
        {
            var userEntity = await _unitOfWork.Users.GetUserByIdAsync(id);
            if (userEntity == null) return null!;
            return userEntity is not null ? new GetUserDTO(
                userEntity.Id,
                userEntity.Name!,
                userEntity.PhoneNumber!,
                userEntity.Email!,
                userEntity.Address!,
                userEntity.City!,
                userEntity.Role!
            ) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return new Response(false, "Email không tồn tại");
            }
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);
            if (!verifyPassword)
            {
                return new Response(false, "Mật khẩu không đúng");
            }
            string token = GenerateToken(user);
            return new Response(true, "Đăng nhập thành công");
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Name),
                new (ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Role, user.Role!),

            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            Console.WriteLine("Generating token for user: " + user.Email);
            Console.WriteLine("User Role: " + user.Role);
            Console.WriteLine("JWT Key: " + _configuration["Authentication:Key"]);
            Console.WriteLine("Token sinh ra: " + token);
             var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine("JWT Token: " + jwt); // 👈 phải in ra được token JWT
            return jwt;

        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(appUserDTO.Email);
            if (user != null)
            {
                return new Response(false, "Email đã được sử dụng");
            }
            var newUser = _unitOfWork.Users.AddUserAsync(new AppUser()
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                PhoneNumber = appUserDTO.PhoneNumber,
                Address = appUserDTO.Address,
                City = appUserDTO.City,
                Role = "Customer", // Mặc định là Customer, có thể thay đổi nếu cần
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password)

            });

            await _unitOfWork.SaveAsync();
            return newUser.Id > 0
                ? new Response(true, "Đăng ký thành công")
                : new Response(false, "Đăng ký thất bại");
        }
    }
}
