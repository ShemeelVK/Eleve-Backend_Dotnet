using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure.Services
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
            bool exists = _context.Products.Any(u => u.Id == product.Id);

            if (exists)
            {
                throw new InvalidOperationException($"A product with ID {product.Id} already exists");
            }

            // Adding the product to the DbContext
            _context.Products.Add(product);

            //saving changes in the sql
            _context.SaveChanges();

            return product;
        }

        public void UpdateProduct(int id,Product updatedProduct)
        {
            var existingProduct = _context.Products.Find(id);
            if (existingProduct != null)
            {
                _context.Entry(existingProduct).CurrentValues.SetValues(updatedProduct);

                //preventing id from being changed
                existingProduct.Id = id;

                _context.SaveChanges();
            }
        }

        public void DeleteProduct(int id)
        {
            var product=_context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public List<ProductDto> GetProductsByCategory(string category)
        {
            return _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured
                })
                .ToList();
        }
    }
}
