namespace Eleve_Backend.Application.Interfaces;
using System.Threading.Tasks;

public interface IUserService
{
    Task<bool> UpdateUserNameAsync(int userId, string newName);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}
