using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.User
{
    public class UpdateNameDto
    {
        [Required]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z0-9\s.]+(?<!\s)(?<!\.)$",
         ErrorMessage = "Username cannot start/end with spaces or dots, and special characters are not allowed.")]
        public string NewName { get; set; }
    }
    public class ChangePasswordDto
    {

        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
           ErrorMessage = "Password must be 8+ chars, with 1 uppercase, 1 lowercase, 1 number, and 1 special char.")]
        public string NewPassword { get; set; }
    }
}
