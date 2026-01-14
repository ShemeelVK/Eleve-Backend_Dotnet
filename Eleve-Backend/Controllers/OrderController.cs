using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.Payment;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Eleve_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly EleveDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IPdfService _pdfService;


        public OrderController(IOrderService orderService, IPdfService pdfService,EleveDbContext context)
        {
            _orderService = orderService;
            _pdfService = pdfService;
            _context = context;
        }

        [HttpPost("Place-Order")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrderAsync([FromBody] CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId=int.Parse(userIdClaim);

            var orderReference=await _orderService.PlaceOrderAsync(userId, dto);

            return Ok(new { Order = orderReference, Message = "Order placed Successfully" });
        }

        [HttpPut("Order-Status/{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {

                var success = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
                if (!success)
                    return NotFound();

                return Ok(new { Message = $"Order status updated to {dto.Status}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-order")]
        [Authorize(Roles ="Customer")] //user
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

        [HttpGet("admin/All-Orders")]
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

        [HttpGet("{orderId}/invoice")]
        [Authorize]
        public async Task<IActionResult> DownloadInvoice(Guid orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var userId=int.Parse(userIdString);

            var order=await _context.Orders
                .Include(o=>o.Items)
                .FirstOrDefaultAsync(o=>o.Id==orderId && o.UserId==userId);

            if (order == null)
            {
                return NotFound("Order Not Found");

            }
            var pdfBytes = _pdfService.GenerateInvoice(order);
            var fileName= $"Invoice_{order.OrderReference}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPut("Return-Order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ReturnOrder(Guid orderId)
        {
            var userIdString=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var userId=int.Parse(userIdString);

            var success = await _orderService.ReturnOrderAsync(orderId, userId);

            if (!success)
            {
                return BadRequest(new { message = "Unable to return order. Ensure the order is Delivered and belongs to you" });
            }

            return Ok(new { message = "Order returned successfully" });
        }
    }
}
