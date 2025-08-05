using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using System.Security.Cryptography;
using System.Text;

namespace MyWebApi.Services
{
    // Fix for CS1722: Base class 'GenericService<AppUser>' must come before any interfaces
    public class UserService : IAppUser
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        public UserService(IUnitOfWork unitOfWork, IUserRepository repository, IConfiguration configuration, IEmailService emailService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
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
            var tokenResult = await _tokenService.GenerateToken(user);
            Console.WriteLine(tokenResult.AccessToken);
            Console.WriteLine(tokenResult.RefreshToken);

            return new Response(true, "Đăng nhập thành công");
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

        public async Task<Response> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByIdAsync(changePasswordDTO.userId);
                if (user == null)
                {
                    return new Response(false, "Người dùng không tồn tại");
                }
                // isActive check : làm thêm sau
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(changePasswordDTO.CurrentPassword, user.Password);
                if (!isPasswordValid)
                {
                    return new Response(false, "Mật khẩu hiện tại không đúng");
                }
                if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
                {
                    return new Response(false, "Mật khẩu mới và xác nhận mật khẩu không khớp");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);
                await _unitOfWork.Users.UpdateUserAsync(user);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Mật khẩu đã được thay đổi thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Lỗi khi thay đổi mật khẩu{ex.Message}");
            }
        }

        public async Task<Response> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(resetPasswordDTO.Email);
                if (user == null)
                {
                    return new Response(false, "Email không tồn tại");
                }
                var newPassword = GeneratePassword();
                string subject = "Mật khẩu mới của bạn";
                string body = $"Mật khẩu mới của bạn là: {newPassword}\nVui lòng đăng nhập và đổi mật khẩu.";
                await _emailService.SendEmailAsync(user.Email!, subject, body);
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                //thêm logic token sau
                await _unitOfWork.Users.UpdateUserAsync(user);
                await _unitOfWork.SaveAsync();

                return new Response(true, "Mật khẩu mới đã được gửi qua email");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Lỗi khi reset mật khẩu{ex.Message}");
            }
        }
        private static string GeneratePassword(int length = 8)
        {

            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(result);

        }
       
    }
}
