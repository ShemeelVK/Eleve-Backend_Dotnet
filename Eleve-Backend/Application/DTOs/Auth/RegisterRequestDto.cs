using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        //regex letters
        [RegularExpression(@"^[a-zA-Z]+(?:\s[a-zA-Z]+)*$", ErrorMessage = "Name must contain only letters and single spaces.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage ="Invalid Email Format")] //this checks for @ and .com
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage ="Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;    
    }
}
