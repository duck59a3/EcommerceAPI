using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAppUser _userService;
        public UserController(IAppUser userService)
        {
            _userService = userService;
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
    }
}
