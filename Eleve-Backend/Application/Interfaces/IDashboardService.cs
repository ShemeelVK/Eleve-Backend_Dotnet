using Eleve_Backend.Application.DTOs.Dashboard;

namespace Eleve_Backend.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardStatsAsync(DateTime startDate, DateTime endDate);
    }
}
