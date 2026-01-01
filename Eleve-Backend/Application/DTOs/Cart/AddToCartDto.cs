using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
