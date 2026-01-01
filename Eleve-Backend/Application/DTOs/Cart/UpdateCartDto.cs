using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Cart
{
    public class UpdateCartDto
    {
        [Required]
        [Range(1,100, ErrorMessage ="Quantity must be atleast 1")]
        public int NewQuantity { get; set; } = 1;
    }
}
