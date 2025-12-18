namespace Eleve_Backend.Domain.Entities
{
    public class WishListItem : BaseEntity
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }

        //Navigatoin properties
        public User? User { get; set; }
        public Product? Product { get; set; }
    }
}
