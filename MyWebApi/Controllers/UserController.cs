using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAppUser _userService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        public UserController(IAppUser userService, IEmailService emailService, ITokenService tokenService)
        {
            _userService = userService;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.Register(appUserDTO);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.Login(loginDTO);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new Response(false, "Người dùng không tồn tại"));
            }
            if (id <= 0)
            {
                return BadRequest(new Response(false, "ID không hợp lệ"));
            }
            return Ok(user);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            if (users == null || !users.Any())
            {
                return NotFound(new Response(false, "Không có người dùng nào"));
            }
            return Ok(users);
        }
        [HttpPut("change-password")]
        public async Task<ActionResult<Response>> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.ChangePassword(changePasswordDTO);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
        [HttpPost("renew-accesstoken")]
        public async Task<ActionResult<Response>> RenewToken(TokenResponseDTO tokenResponseDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _tokenService.RenewToken(tokenResponseDTO);

            return response.Flag ? Ok(response) : BadRequest(response);
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<Response>> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.ResetPassword(resetPasswordDTO);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
        
}
