using Microsoft.AspNetCore.Mvc;
using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleve_Backend.Application.DTOs.User;

namespace Eleve_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpPut("Update-Name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateNameDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId=int.Parse(userIdClaim);

            var success = await _userService.UpdateUserNameAsync(userId, dto.NewName);

            if (!success)
                return BadRequest("Failed to update name");

            return Ok(new { Message = "Name updated Successfully" });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            try
            {
               var success = await _userService.ChangePasswordAsync(int.Parse(userIdClaim), dto.CurrentPassword, dto.NewPassword);

                return Ok(new { Message = "Password changed successfully" });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error eccured while changing the password");
            }

  
        }
        
    }
}
