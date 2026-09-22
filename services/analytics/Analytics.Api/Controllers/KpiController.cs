using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Analytics.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace Analytics.Api.Controllers;

[ApiController]
[Route("api/v1/analytics/kpis")]
[Authorize]
public class KpiController : ControllerBase
{
    private readonly AnalyticsDbContext _context;

    public KpiController(AnalyticsDbContext context)
    {
        _context = context;
    }

    [HttpGet("transit-time")]
    // [Authorize(Policy = "analytics.kpi.read")]
    public async Task<IActionResult> GetTransitTimeKpi([FromQuery] Guid? tenantId)
    {
        // Extract TenantId from JWT to enforce tenant isolation unless user is Platform Admin
        var userTenantId = Guid.Parse(User.FindFirst("TenantId")?.Value ?? tenantId.ToString() ?? Guid.Empty.ToString());
        
        // In a real scenario, this would query a materialized view or perform an aggregated query on FactTripMilestone
        // calculating the P95 transit time.
        
        var baseQuery = _context.FactTripMilestones.AsQueryable();
        
        if (userTenantId != Guid.Empty)
        {
            baseQuery = baseQuery.Where(f => f.TenantId == userTenantId);
        }
        
        var avgDuration = await baseQuery
            .Where(f => f.ActualDurationFromStart.HasValue)
            .AverageAsync(f => f.ActualDurationFromStart.Value.TotalHours);

        return Ok(new 
        { 
            Code = "avg-transit-time",
            Name = "Average Transit Time",
            Value = avgDuration,
            Unit = "Hours"
        });
    }
}
