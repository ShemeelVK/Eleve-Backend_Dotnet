using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Eleve_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var token=_authService.Login(request);

            if (token == null)
                return Unauthorized("Invalid Credentials");

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto request)
        {
            var user=_authService.Register(request);

            if (user == null)
            {
                return BadRequest("User already exists");
            }
            return Ok(new { message = "User registered successfully", userId = user.Id });
        }
    }
}
