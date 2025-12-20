using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Eleve_Backend.Infrastructure.Persistence;


namespace Eleve_Backend.Infrastructure.Services
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

        public void RemoveFromCart(int userId,int productId)
        {
            var item = _context.CartItems.FirstOrDefault(s => s.UserId == userId&& s.ProductId == productId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
        }

        public void ClearCart(int userId)
        {
            var items = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(items);
            _context.SaveChanges();
        }

        public void UpdateQuantity(int userId,int productId,int newQuantity)
        {
            var item = _context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                if(newQuantity > 0)
                {
                    item.Quantity = newQuantity;
                }
                else
                {
                    _context.CartItems.Remove(item);
                }
            }
            _context.SaveChanges();
        }
    }
}
