using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Products
{
    public class CreateProductDto
    {
        [Required(ErrorMessage ="Product Name is required")]
        [StringLength(100,MinimumLength =3)]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z0-9\s.]+(?<!\s)(?<!\.)$",
         ErrorMessage = "Name cannot start/end with spaces or dots, and must only contain letters, numbers, and spaces.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300,MinimumLength =3)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01,100000, ErrorMessage ="Price must be greater than 0")]
        public decimal Price { get; set; }
        

        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Stock must be at least 1")]
        public int Stock { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Category can only contain letters.")]
        public string Category { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }
    }
}
