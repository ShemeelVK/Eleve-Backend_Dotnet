using Microsoft.AspNetCore.Mvc;

namespace Eleve_Backend.Application.DTOs.User
{
    public class UserSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
