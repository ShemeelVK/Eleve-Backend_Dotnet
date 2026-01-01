using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Eleve_Backend.Application.DTOs.Wishlist;

namespace Eleve_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId()
        {
            var idClaim=User.FindFirst(ClaimTypes.NameIdentifier);
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }

        [HttpGet("Get All Products")]
        public IActionResult GetMyWishlist()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var wishlist=_wishlistService.GetMyWishlist(userId);

            if(wishlist==null || wishlist.Count == 0)
            {
                return Ok(new List<WishlistDto>()); //return empty list
            }

            return Ok(wishlist);
        }

        [HttpPost("Toogle Wishlist")]
        public IActionResult Toggle(int productId)
        {
            var userId=GetUserId();
            var result = _wishlistService.ToggleWishlist(userId, productId);

            return Ok(new { message = result });
        }

        [HttpDelete("Clear Wishlist")]
        public IActionResult Clear()
        {
            var userId= GetUserId();
            _wishlistService.ClearWishlist(userId);
            return Ok("Wishlist cleared");
        }
    }
}
