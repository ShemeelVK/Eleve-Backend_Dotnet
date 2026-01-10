using Eleve_Backend.Application.DTOs.Auth;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.User;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Domain.ValueObjects;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            if (!isOldPasswordCorrect)
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

        public async Task<bool> AddAddressAsync(int userId,AddressDto addressDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user==null) return false;

            var newAddress = new Address(
                addressDto.Name,
                addressDto.Street,
                addressDto.City,
                addressDto.State,
                addressDto.ZipCode,
                addressDto.PhoneNumber
            );

            user.SavedAddresses.Add(newAddress);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AddressDto>> GetUserAddressesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.SavedAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(user==null) return new List<AddressDto>();

            return user.SavedAddresses.Select(a => new AddressDto
            {
                Name = a.Name,
                Street = a.Street,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                PhoneNumber = a.PhoneNumber
            }).ToList();
        }

        public async Task<IEnumerable<UserSearchDto>> GetAllUsersAsync(string? searchTerm)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower().Trim();

                query = query.Where(u =>
                u.Username.ToLower().Contains(lowerSearch) ||
                u.Email.ToLower().Contains(lowerSearch)
                );
            }

            //selecting and execute
            var users=await query
                .OrderByDescending(u => u.Id) //new people first
                .Take(10)
                .Select(o => new UserSearchDto
                {
                    Id= o.Id,
                    Name=o.Username,
                    Email=o.Email,
                    Role=o.Role,
                    IsActive = o.IsActive
                }).ToListAsync();

            return users;
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (user.Role == "Admin")
            {
                throw new InvalidOperationException("Cannot block an Admin");
            }

            user.IsActive = !user.IsActive;

            await _context.SaveChangesAsync();

            return user.IsActive;
        }
    }
}
