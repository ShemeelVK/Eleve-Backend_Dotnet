using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Eleve_Backend.Infrastructure
{
    public class CartService : ICartService
    {
        private readonly EleveDbContext _context;

        public CartService(EleveDbContext context)
        {
            _context = context;
        }

        public List<CartItem> GetCart(int userId)
        {
            //"include" to laod the product details
            return _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public void AddToCart(int userId,int productId,int quantity)
        {
            //checking if item already exists in the cart
            var existingitem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if(existingitem != null)
            {
                //increase quantity if item exists
                existingitem.Quantity += quantity;
            }
            else
            {
                //adding new item
                var newitem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };

                _context.CartItems.Add(newitem);
            }

            _context.SaveChanges();
        }

        public void RemoveFromCart(int userId,int cartItemId)
        {
            var item = _context.CartItems.FirstOrDefault(s => s.Id == cartItemId && s.UserId == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
