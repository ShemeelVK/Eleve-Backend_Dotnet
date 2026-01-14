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

        //Helper method
        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortOrder)
        {
            if(string.IsNullOrEmpty(sortOrder))
                return query;

            switch (sortOrder.ToLower())
            {
                case "lowtohigh":
                    return query.OrderBy(p => p.Price);
                case "hightolow":
                    return query.OrderByDescending(p => p.Price);

                default:
                    return query;
            }
        }

        public List<ProductDto> GetAllProducts(string? sortOrder = null)
        {
            var query = _context.Products.Where(p => !p.IsDeleted);
            query=ApplySorting(query, sortOrder);

            return query.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                Stock = p.Stock
            }).ToList();
        }

        public List<ProductDto> GetFeaturedProduct()
        {
            return _context.Products
                .Where(p => p.IsFeatured == true && !p.IsDeleted)
                .Select(s=>new ProductDto
                {
                    Id=s.Id,
                    Name=s.Name,
                    Description=s.Description,
                    Price=s.Price,
                    Category=s.Category,
                    ImageUrl=s.ImageUrl,
                    IsFeatured=s.IsFeatured,
                    Stock=s.Stock
                })
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product AddProduct(Product product)
        {
            bool exists = _context.Products.Any(u => u.Name.ToLower() == product.Name.ToLower());

            if (exists)
            {
                throw new InvalidOperationException($"A product with {product.Name} already exists");
            }
      
            _context.Products.Add(product);

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
                product.IsDeleted = true;
                _context.SaveChanges();
            }
        }

        public List<ProductDto> GetProductsByCategory(string category,string? sortOrder=null)
        {
            var query = _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .Where(p => !p.IsDeleted);

            query = ApplySorting(query, sortOrder);

            return query.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                Stock = p.Stock
            }).ToList();

        }

        public async Task<List<ProductDto>> SearchProductsAsync(string query, string? sortOrder = null, bool includeDeleted = false)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<ProductDto>();

            var lowerQuery = query.ToLower();

            var dbQuery=_context.Products.AsQueryable();

            if (!includeDeleted)
            {
                dbQuery = dbQuery.Where(p => !p.IsDeleted);
            }

            dbQuery = dbQuery
                .Where(p => p.Name.ToLower().Contains(lowerQuery) ||
                  p.Description.ToLower().Contains(lowerQuery));

            dbQuery=ApplySorting(dbQuery, sortOrder);

            return await dbQuery.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                Stock = p.Stock,
                IsDeleted = p.IsDeleted
            }).ToListAsync();

        }
    }
}
