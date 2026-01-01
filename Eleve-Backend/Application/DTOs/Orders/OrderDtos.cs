using Eleve_Backend.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Eleve_Backend.Application.DTOs.Orders
{
    public class UpdateStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
