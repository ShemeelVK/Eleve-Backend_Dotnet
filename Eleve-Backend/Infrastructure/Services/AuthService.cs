using BCrypt.Net;
using Eleve_Backend.Application.DTOs.Auth;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Eleve_Backend.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly EleveDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EleveDbContext context,IConfiguration configuration)
        {
            _context= context;
            _configuration= configuration;
        }

        public async Task<string?> GeneratePasswordResetOtp(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(1);

            await _context.SaveChangesAsync();
            return otp;
        }

        public async Task<bool> ResetPasswordWithOtp(string email,string otp,string newPassword)
        {
            var user=await _context.Users.FirstOrDefaultAsync(s => s.Email == email);

            if(user == null || user.ResetOtp != otp || user.OtpExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetOtp = null;
            user.OtpExpiryTime = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> Register(RegisterRequestDto request)
        {
            //checking if email exists
            if(await _context.Users.AnyAsync(u=> u.Email == request.Email))
            {
                throw new Exception("Email already exists");
            }

            //hashing the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "Customer",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Registered successfully";
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            //find the user using email
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Email==request.Email);

            if (user == null)
                throw new Exception("Invalid email or Password");

            //check if blocked
            if (!user.IsActive)
                throw new Exception("Your account has been blocked");

            //verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Invalid email or password");

            var token= GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken=refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Username,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        public async Task<LoginResponseDto> RefreshToken(string oldRefreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == oldRefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired refresh token");
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,

                User = new UserDto { Id = user.Id, Name = user.Username, Email = user.Email, Role = user.Role }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key= Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role)
                }),

                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
