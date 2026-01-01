using Eleve_Backend.Application.DTOs.Wishlist;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly EleveDbContext _context;

        public WishlistService(EleveDbContext context)
        {
            _context = context;
        }

        public List<WishlistDto> GetMyWishlist(int userId)
        {
            //join logic: get wishlist => Join Product => Flatten into Db
            return _context.WishItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .Select(w => new WishlistDto
                {
                    Id = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Product.Name,
                    Price = w.Product.Price,
                    ImageUrl = w.Product.ImageUrl,
                    IsFeatured=w.Product.IsFeatured
                })
                .ToList();
        }

        public string ToggleWishlist(int userId,int productId)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
                throw new KeyNotFoundException("Product Not Found");

            if (product.Stock <= 0)
                throw new InvalidOperationException("Cannot add out-of-stock item to wishlist");


            var item = _context.WishItems
                .FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);

            if (item != null)
            {
                _context.WishItems.Remove(item);
                _context.SaveChanges();
                return "Removed from wishlist";
            }
            else
            {
                var newitem = new WishListItem
                {
                    UserId = userId,
                    ProductId = productId
                };

                _context.WishItems.Add(newitem);
                _context.SaveChanges();
                return  "Added";
            }
        }

        public void ClearWishlist(int userId)
        {
            var items = _context.WishItems.Where(w => w.UserId == userId);
            _context.WishItems.RemoveRange(items);
            _context.SaveChanges();
        }




        //public void RemoveFromWishlist(int userId, int productId)
        //{
        //    var item = _context.WishItems
        //        .FirstOrDefault(W => W.UserId == userId && W.ProductId == productId);

        //    if (item != null)
        //    {
        //        _context.WishItems.Remove(item);
        //        _context.SaveChanges();
        //    }
        //}
    }
}
