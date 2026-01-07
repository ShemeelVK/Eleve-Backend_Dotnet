using System;
using System.Threading.Tasks;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Eleve_Backend.Infrastructure.Persistence;

namespace Eleve_Backend.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly EleveDbContext _context;

        public UserService(EleveDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateUserNameAsync(int userId, string newName)
        {
            var user=await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.Username = newName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);

            if (isOldPasswordCorrect)
                throw new ArgumentException("The current password you entered is incorrect");  //old password didnt match

            if (currentPassword == newPassword)
            {
                throw new ArgumentException("New password cannot be the same as the old password");
            }

            string newPasswordHash=BCrypt.Net.BCrypt.HashPassword(newPassword);

            user.PasswordHash = newPasswordHash;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
