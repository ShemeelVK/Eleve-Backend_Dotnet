using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure;
using Microsoft.Extensions.Configuration; //needed for appsettings
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eleve_Backend.Infrastructure
{
    public class JwtAuthService : IAuthService
    {
        private readonly EleveDbContext _context; 

        private readonly IConfiguration _configuration;

        public JwtAuthService(EleveDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string? Login(LoginRequestDto request)
        {
            //Check user first
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.PasswordHash == request.Password);
            if (user == null || user.PasswordHash != request.Password)
                return null;

            //generate token (infrastructure logic)
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Email, user.Email),

                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User? Register(RegisterRequestDto request)
        {
            if(_context.Users.Any(u=>u.Username == request.Username || u.Email==request.Email))
            {
                return null;
            }

            var newuser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                Role = "User"
            };

            _context.Users.Add(newuser);
            _context.SaveChanges(); // this is used to commit the changes to the sql

            return newuser;
        }
    }
}
