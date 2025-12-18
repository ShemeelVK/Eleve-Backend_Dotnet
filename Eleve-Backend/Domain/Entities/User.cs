namespace Eleve_Backend.Domain.Entities
{
    public class User : BaseEntity
    {
        //public int Id { get; set; }  used baseentity
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";

    }
}
