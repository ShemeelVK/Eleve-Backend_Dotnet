using Eleve_Backend.Domain.ValueObjects;
using System.Collections.Generic;
using Eleve_Backend.Domain.Enums;
namespace Eleve_Backend.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderReference { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public Decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        public Address ShippingAddress { get; set; }
        public string? TransactionId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
