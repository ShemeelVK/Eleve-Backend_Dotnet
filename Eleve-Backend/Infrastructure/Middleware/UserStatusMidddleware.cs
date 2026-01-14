using System.Security.Claims;
using Eleve_Backend.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            
            if (context.User.Identity?.IsAuthenticated == true)
            {
                
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<EleveDbContext>();

                        
                        var user = await dbContext.Users
                            .AsNoTracking()
                            .Select(u => new { u.Id, u.IsActive }) 
                            .FirstOrDefaultAsync(u => u.Id == userId);

                        
                        if (user != null && !user.IsActive)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsJsonAsync(new { message = "Your account has been suspended. Please contact support." });
                            return; // Stop the pipeline here. Do not call _next
                        }
                    }
                }
            }

            
            await _next(context);
        }
    }
}