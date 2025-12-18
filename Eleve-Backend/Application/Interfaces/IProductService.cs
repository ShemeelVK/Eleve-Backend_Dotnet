using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Application.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product? GetProductById(int id);
        Product AddProduct(Product product);
    }
}
