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
        [HttpGet("Cart-Products")]
        public IActionResult GetCartItems()
        {
            var userId=GetUserId();
            List<CartItemDto> cart = _cartService.GetCart(userId);
            return Ok(cart);
        }

        //adding items to cart
        [HttpPost("Add-To-Cart")]
        public IActionResult AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                var userId=GetUserId() ;
                _cartService.AddToCart(userId, request.ProductId, request.Quantity);
                return Ok("Item added to cart");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal error occured" });
            }
        }

        //updating cart(quantity)
        [HttpPut("Product-Quantity")]
        public IActionResult UpdateQuantity( int itemId , [FromBody] UpdateCartDto request)
        {
            try
            {

                var userId = GetUserId();
                _cartService.UpdateQuantity(userId, itemId, request.NewQuantity);
                return Ok("Cart quantity updated");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        //removing from cart
        [HttpDelete("Remove-from-Cart")]
        public IActionResult RemoveFromCart(int itemId)
        {
            try
            {
                var userId=GetUserId();
                _cartService.RemoveFromCart(userId, itemId);
                return Ok("Item removed");
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        //clearing cart
        [HttpDelete("Clear-Cart")]
        public IActionResult ClearCart()
        {
            var userId = GetUserId();
            _cartService.ClearCart(userId);
            return Ok("Cart Cleared");
        }

    }
}
