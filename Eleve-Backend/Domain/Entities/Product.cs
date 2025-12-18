namespace Eleve_Backend.Domain.Entities
{
    public class Product : BaseEntity
    {
        //public int Id { get; set; }  used baseentity
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; }= string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Stock {  get; set; }
        public string ImageUrl { get; set; }= string.Empty;
    }
}
