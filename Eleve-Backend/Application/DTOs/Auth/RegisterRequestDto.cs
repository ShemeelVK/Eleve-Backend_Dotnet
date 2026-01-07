using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        //regex letters
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z0-9\s.]+(?<!\s)(?<!\.)$",
         ErrorMessage = "Username cannot start/end with spaces or dots, and special characters are not allowed.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage ="Invalid Email Format")] //this checks for @ and .com
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email cannot contain spaces.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage ="Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be 8+ chars, with 1 uppercase, 1 lowercase, 1 number, and 1 special char.")]
        public string Password { get; set; } = string.Empty;    
    }
}
