using Eleve_Backend.Application.DTOs.Orders;

namespace Eleve_Backend.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<OrderResponseDto> Orders { get; set; } = new List<OrderResponseDto>();
    }
}
