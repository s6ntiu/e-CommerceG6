using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using User.API.DTOs;
using User.API.Services;

namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var newUser = await _userService.RegisterAsync(request);

            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, new
            {
                newUser.Id,
                newUser.Nombre,
                newUser.Apellido,
                newUser.Email,
                newUser.FechaRegistration,
                newUser.Activo
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request);

            return Ok(new
            {
                user.Id,
                user.Nombre,
                user.Apellido,
                user.Email
            });
        }
    }
}