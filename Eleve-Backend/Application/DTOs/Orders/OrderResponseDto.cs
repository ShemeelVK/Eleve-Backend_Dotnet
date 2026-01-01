using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Eleve_Backend.Application.DTOs;

namespace Eleve_Backend.Application.DTOs.Orders
{
    public class OrderResponseDto 
    {
        public Guid Id { get; set;}
        public DateTime OrderDate { get; set;}
        public decimal TotalAmount { get; set;}
        public string Status {  get; set;} //returning status as string to frontend
        public AddressDto ShippingAddress { get; set;}
        public List<OrderItemResponseDto> Items { get; set;}
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set;}
        public string ProductName { get; set;}
        public decimal UnitPrice { get; set;}
        public int Quantity { get; set;}
        public string ProductImage { get; set;}
    }
}
