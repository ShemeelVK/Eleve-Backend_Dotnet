using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

namespace Eleve_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //helper method to get user ID from the token
        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                return 0;
            return int.Parse(idClaim.Value);
        }
        //getting all products
        [HttpGet("Get all Products")]
        public IActionResult GetCartItems()
        {
            var userId=GetUserId();
            var items = _cartService.GetCart(userId);
            return Ok(items);
        }

        //adding items to cart
        [HttpPost("Add To Cart")]
        public IActionResult AddToCart([FromBody] AddToCartDto request)
        {
            var userId=GetUserId() ;
            _cartService.AddToCart(userId, request.ProductId, request.Quantity);
            return Ok("Item added to cart");
        }

        //removing from cart
        [HttpDelete("Remove from Cart")]
        public IActionResult RemoveFromCart(int itemId)
        {
            var userId=GetUserId();
            _cartService.RemoveFromCart(userId, itemId);
            return Ok("Item removed");
        }

    }
}
