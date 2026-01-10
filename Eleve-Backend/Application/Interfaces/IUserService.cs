namespace Eleve_Backend.Application.Interfaces;

using Eleve_Backend.Application.DTOs.Auth;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.User;
using System.Threading.Tasks;

public interface IUserService
{
    Task<bool> UpdateUserNameAsync(int userId, string newName);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> AddAddressAsync(int userId, AddressDto addressDto);
    Task<List<AddressDto>> GetUserAddressesAsync(int userId);
    Task<IEnumerable<UserSearchDto>> GetAllUsersAsync(string? searchTerm);
    Task<bool> ToggleUserStatusAsync(int userId);
}
