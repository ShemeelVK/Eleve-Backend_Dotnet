using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Eleve_Backend.Application.DTOs.Cart;

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
        public IActionResult AddToCart([FromForm] AddToCartDto request)
        {
            var userId=GetUserId() ;
            _cartService.AddToCart(userId, request.ProductId, request.Quantity);
            return Ok("Item added to cart");
        }

        //updating cart(quantity)
        [HttpPut("Product Quantity")]
        public IActionResult UpdateQuantity(int itemId, [FromBody] UpdateCartDto request)
        {
            var userId = GetUserId();
            _cartService.UpdateQuantity(userId, itemId, request.NewQuantity);
            return Ok("Cart quantity updated");
        }

        //removing from cart
        [HttpDelete("Remove from Cart")]
        public IActionResult RemoveFromCart(int itemId)
        {
            var userId=GetUserId();
            _cartService.RemoveFromCart(userId, itemId);
            return Ok("Item removed");
        }

        //clearing cart
        [HttpDelete("Clear Cart")]
        public IActionResult ClearCart()
        {
            var userId = GetUserId();
            _cartService.ClearCart(userId);
            return Ok("Cart Cleared");
        }

    }
}
