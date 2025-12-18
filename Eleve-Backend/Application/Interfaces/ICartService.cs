using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Application.Interfaces
{
    public interface ICartService
    {
        //get all items
        List<CartItem> GetCart(int userId);

        //adding item
        void AddToCart(int userId, int productId, int quantity);

        //remove item
        void RemoveFromCart(int userId, int cartItemId);
    }
}
