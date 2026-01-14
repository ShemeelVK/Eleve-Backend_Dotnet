using Eleve_Backend.Application.DTOs.Auth;
using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
namespace Eleve_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IUserService userService, IEmailService emailService)
        {
            _authService = authService;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var otp = await _authService.GeneratePasswordResetOtp(request.Email);
            if (otp == null) return NotFound("User not found.");

            string body = $@"
        <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 500px; margin: auto; border: 1px solid #e0e0e0; border-radius: 12px; padding: 40px; color: #333;'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <h1 style='color: #4f46e5; margin: 0; font-size: 28px; letter-spacing: 2px;'>ELEVÉ</h1>
                <p style='color: #666; font-size: 12px; text-transform: uppercase;'>Luxury Sneaker Store</p>
            </div>
            
            <h3 style='font-size: 18px; color: #111;'>Password Reset Request</h3>
            <p>Hello,</p>
            <p>We received a request to reset the password for your Elevé account. Use the verification code below to proceed:</p>
            
            <div style='background-color: #f3f4f6; padding: 20px; text-align: center; border-radius: 8px; margin: 25px 0;'>
                <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #4f46e5;'>{otp}</span>
            </div>
            
            <p style='font-size: 13px; color: #666;'>This code is valid for <b>2 minutes</b>. If you did not request this, please ignore this email or contact our support team.</p>
            
            <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;'>
            
            <p style='font-size: 12px; color: #999; text-align: center;'>
                Regards,<br>
                <strong>The Elevé Team</strong><br>
                Elevé Luxury Sneakers, Inc.
            </p>
        </div>";

            bool emailSent = await _emailService.SendEmailAsync(request.Email, "Reset Your Elevé Password - Action Required", body);

            return emailSent ? Ok("OTP sent.") : StatusCode(500, "Email error.");
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var success = await _authService.ResetPasswordWithOtp(request.Email, request.Otp, request.NewPassword);

            if (!success)
                return BadRequest("Invalid or expired OTP");

            return Ok("Password reset successfully");
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.Login(request);

                SetRefreshTokenCookie(result.RefreshToken);

                return Ok(new
                {
                    token=result.Token,
                    user=result.User
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new {message="Logged out successfully"});
        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { error = "No refresh token provided" });

                var result = await _authService.RefreshToken(refreshToken);

                SetRefreshTokenCookie(result.RefreshToken);

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
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

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,    // JS cannot access this (Prevents XSS theft)
                Expires = DateTime.UtcNow.AddDays(7),
                //SameSite = SameSiteMode.Lax, // Protects against CSRF
                Secure = true,      // Set to true if you are using HTTPS (Recommended)
                SameSite = SameSiteMode.None,
                Path="/"
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
