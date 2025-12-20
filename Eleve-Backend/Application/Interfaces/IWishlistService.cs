using Eleve_Backend.Application.DTOs.Wishlist;

namespace Eleve_Backend.Application.Interfaces
{
    public interface IWishlistService
    {
        List<WishlistDto> GetMyWishlist(int userId);
        string ToggleWishlist(int userId, int productId);
        void ClearWishlist(int userId);
    }
}
