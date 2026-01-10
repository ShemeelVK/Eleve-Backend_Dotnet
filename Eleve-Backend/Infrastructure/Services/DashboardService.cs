using Eleve_Backend.Application.DTOs.Dashboard;
using Eleve_Backend.Application.DTOs.Orders;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly EleveDbContext _context;

        public DashboardService(EleveDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync(DateTime startDate, DateTime endDate)
        {
            var totalUsers = await _context.Users
                .Where(u => u.Role == "Customer")
                .CountAsync();

            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);

            var orders = await _context.Orders
             .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
             .Include(o => o.ShippingAddress)
             .OrderByDescending(o => o.OrderDate)
             .Select(o => new OrderResponseDto
             {
                 Id = o.Id,
                 OrderReference = o.OrderReference,
                 OrderDate = o.OrderDate,
                 TotalAmount = o.TotalAmount,
                 Status = o.Status.ToString(),
                 PaymentMethod = o.PaymentMethod,
                 ShippingAddress = new AddressDto
                 {
                     Name = o.ShippingAddress.Name,
                     Street = o.ShippingAddress.Street,
                     City = o.ShippingAddress.City,
                     State = o.ShippingAddress.State,
                     ZipCode = o.ShippingAddress.ZipCode,
                     PhoneNumber = o.ShippingAddress.PhoneNumber
                 },
                 Items = new List<OrderItemResponseDto>() // Empty list for performance
             })
             .ToListAsync();

            return new DashboardDto
            {
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                Orders = orders
            };
        }

    }
}
