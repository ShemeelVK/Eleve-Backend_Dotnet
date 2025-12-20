using Eleve_Backend.Application.DTOs.Auth;
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
        public async Task<IActionResult> Login([FromForm] LoginRequestDto request)
        {
            try
            {
                var token = await _authService.Login(request);
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.Register(request);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
