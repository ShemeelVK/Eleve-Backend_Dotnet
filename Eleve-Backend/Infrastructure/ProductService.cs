using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Infrastructure
{
    public class ProductService: IProductService
    {
        private readonly EleveDbContext _context;

        public ProductService(EleveDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product AddProduct(Product product)
        {
            // Adding the product to the DbContext
            _context.Products.Add(product);

            //saving changes in the sql
            _context.SaveChanges();

            return product;


        }
    }
}
