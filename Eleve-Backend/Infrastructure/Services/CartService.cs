using Eleve_Backend.Application.DTOs.Cart;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Eleve_Backend.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly EleveDbContext _context;

        public CartService(EleveDbContext context)
        {
            _context = context;
        }

        public List<CartItemDto> GetCart(int userId)
        {
            //"include" to laod the product details
           return _context.CartItems
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name, 
                Price = c.Product.Price, 
                Category= c.Product.Category,
                ImageUrl = c.Product.ImageUrl,
                Quantity = c.Quantity
            })
            .ToList();
        }

        public void AddToCart(int userId,int productId,int quantity)
        {

            if (quantity <= 0) quantity = 1;

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException("Product Not Found");

            if (product.Stock <= 0)
                throw new InvalidOperationException("Product is out of stock");
          

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            int currentCartQuantity=cartItem!=null ? cartItem.Quantity : 0;
            int totalRequestedQuantity=currentCartQuantity + quantity;

            if (totalRequestedQuantity > product.Stock)
            {
                throw new InvalidOperationException($"Cannot add {quantity} more. you already have {currentCartQuantity} in cart. only {product.Stock} in stock");
            }

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            _context.SaveChanges();
        }

        public void RemoveFromCart(int userId,int productId)
        {
            var item = _context.CartItems.FirstOrDefault(s => s.UserId == userId&& s.ProductId == productId);

            if (item == null)
            {
                throw new KeyNotFoundException("Item doesnot exist in the cart");
            }

            _context.CartItems.Remove(item);
            _context.SaveChanges();
        }

        public void ClearCart(int userId)
        {
            var items = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(items);
            _context.SaveChanges();
        }

        public void UpdateQuantity(int userId,int productId,int newQuantity)
        {
            var item = _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item == null)
            
                throw new KeyNotFoundException("Item not found in the cart");

            if (newQuantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
               if (newQuantity > item.Product.Stock)
                {
                    throw new InvalidOperationException($"We only have {item.Product.Stock} units in stock. You cannot order {newQuantity}");
                }

                item.Quantity = newQuantity;
            }

                _context.SaveChanges();
        }
    }
}
