using System.ComponentModel.DataAnnotations;

namespace Eleve_Backend.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public List<OrderItemDto> Items { get; set; }
        public AddressDto ShippingAddress { get; set; }
    }
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
