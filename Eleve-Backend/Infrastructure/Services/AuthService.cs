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

        public async Task<String> Login(LoginRequestDto request)
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

            return GenerateJwtToken(user);
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
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
