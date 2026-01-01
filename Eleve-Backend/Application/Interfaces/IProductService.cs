using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Application.Interfaces
{
    public interface IProductService
    {
        List<ProductDto> GetAllProducts();
        Product? GetProductById(int id);

        List<ProductDto> GetFeaturedProduct();
        Product AddProduct(Product product);

        void UpdateProduct(int id, Product product);
        void DeleteProduct(int id);
        List<ProductDto> GetProductsByCategory(string category);
    }
}
