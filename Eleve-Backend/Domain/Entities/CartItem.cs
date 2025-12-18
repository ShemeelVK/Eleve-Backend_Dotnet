namespace Eleve_Backend.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        //Navigation Properties
        public User? User { get; set; }
        public Product? Product { get; set; }
    }
}
