using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
