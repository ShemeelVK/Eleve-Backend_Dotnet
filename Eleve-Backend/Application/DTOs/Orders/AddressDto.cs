using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Orders
{
    public class AddressDto
    {
        [Required]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z0-9\s.]+(?<!\s)(?<!\.)$", ErrorMessage = "Invalid Name format.")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z0-9\s.,-]+(?<!\s)(?<!\.)$",
         ErrorMessage = "Street Address has invalid format.")]
        public string Street { get; set; }
        [Required]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z\s]+(?<!\s)(?<!\.)$", ErrorMessage = "City should only contain letters.")]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^(?!\s)(?!\.)[a-zA-Z\s]+(?<!\s)(?<!\.)$", ErrorMessage = "State should only contain letters.")]
        public string State { get; set; }
        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Zip Code must be exactly 6 digits")]
        public string ZipCode { get; set; }
        [Required]
        [Phone(ErrorMessage ="Invalid Phone Number Format")]
        [StringLength(10,ErrorMessage ="Phone Number must be 10 numbers")]
        public string PhoneNumber { get; set; }
    }
}
