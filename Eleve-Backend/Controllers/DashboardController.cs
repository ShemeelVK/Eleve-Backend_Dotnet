using Eleve_Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Eleve_Backend.Application.DTOs;
using Eleve_Backend.Infrastructure.Services;
using Eleve_Backend.Domain.Entities;

namespace Eleve_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("Stats")]
        public async Task<IActionResult> GetDashboardStatsAsync([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var stats = await _dashboardService.GetDashboardStatsAsync(start, end);

            return Ok(stats);
        }
    }
}
