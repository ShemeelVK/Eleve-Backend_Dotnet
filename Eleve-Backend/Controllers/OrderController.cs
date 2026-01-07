using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.Payment;

namespace Eleve_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("Place-Order")]
        [Authorize]
        public async Task<IActionResult> PlaceOrderAsync([FromBody] CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId=int.Parse(userIdClaim);

            var orderReference=await _orderService.PlaceOrderAsync(userId, dto);

            return Ok(new { Order = orderReference, Message = "Order placed Successfully" });
        }

        [HttpPut("{id}/Order-Status")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, dto.Status);

            if (!success)
                return NotFound("Order not Found");

            return Ok(new { Message = $"Order status updated to {dto.Status}" });
        }

        [HttpGet("my-order")]
        [Authorize] //user
        public async Task<IActionResult> GetMyOrders()
        {
            //extracting user id from token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if(string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId= int.Parse(userIdClaim);

            //get the data
            var orders=await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPut("Cancel-Order/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrderAsync(Guid id)
        {
            //current userId from the token
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId=int.Parse(userIdString);

            var success = await _orderService.CancelOrderAsync(id, userId);

            if (!success)
                return BadRequest("Unable to cancel order");

            return Ok(new { Message = "Order cancelled Successfully" });

        }

        [HttpGet("admin/all-orders")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders= await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost("verify_payment")]
        [Authorize]
        public IActionResult VerifyPayment([FromBody] PaymentVerificationDto dto)
        {
            var isValid=_orderService.VerifyPayment(dto);

            if (!isValid)
                return BadRequest(new { Message = "Payment Verification failed!" });

            return Ok(new { Message = "Payment verified Suucessfully" });
        }
    }
}
