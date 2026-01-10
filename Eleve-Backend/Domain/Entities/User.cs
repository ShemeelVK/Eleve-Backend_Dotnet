using Eleve_Backend.Domain.ValueObjects;

namespace Eleve_Backend.Domain.Entities
{
    public class User : BaseEntity
    {
        //public int Id { get; set; }  used baseentity
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public bool IsActive { get; set; } = true;
        public string? RefreshToken { get; set; }
        public string? ResetOtp {  get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<Address> SavedAddresses { get; set; } = new List<Address>();


    }
}
