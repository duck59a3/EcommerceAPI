using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IAppUser 
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUserById(int id);
        Task<IEnumerable<GetUserDTO>> GetAllUsers();
        Task<Response> ChangePassword(ChangePasswordDTO changePasswordDTO);
        Task<Response> ResetPassword(ResetPasswordDTO resetPasswordDTO);

    }
}
