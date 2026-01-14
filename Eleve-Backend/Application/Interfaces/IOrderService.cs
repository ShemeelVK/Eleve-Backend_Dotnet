using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.DTOs.Payment;
using Eleve_Backend.Domain.Enums;

namespace Eleve_Backend.Application.Interfaces
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(int userId, CreateOrderDto dto);
        //Task<bool> ShipOrderAsync(Guid orderId);

        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        Task<List<OrderResponseDto>> GetOrdersByUserIdAsync(int userId);

        //For admin
        Task<List<OrderResponseDto>> GetAllOrdersAsync();

        Task<bool> CancelOrderAsync (Guid orderId,int userId);

        bool VerifyPayment(PaymentVerificationDto dto);
        Task<bool> ReturnOrderAsync(Guid orderId, int userId);
    }
}
