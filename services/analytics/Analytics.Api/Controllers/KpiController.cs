using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Analytics.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace Analytics.Api.Controllers;

/// <summary>
/// A01: reads the tenant exclusively from the JWT (<c>tenant_id</c> claim). The previous version
/// silently fell back to the caller supplied <c>tenantId</c> query value whenever the claim name
/// did not match, which allowed any authenticated user to read another tenant's KPI.
/// </summary>
[ApiController]
[Route("api/v1/analytics/kpis")]
[Authorize(Policy = "analytics.kpi.read")]
public class KpiController : ControllerBase
{
    private readonly AnalyticsDbContext _context;

    public KpiController(AnalyticsDbContext context)
    {
        _context = context;
    }

    [HttpGet("transit-time")]
    public async Task<IActionResult> GetTransitTimeKpi([FromQuery] Guid? tenantId)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var userTenantId)) return Forbid();

        // A requested tenant may only be the caller's own tenant (or empty, meaning "mine").
        if (tenantId.HasValue && tenantId.Value != userTenantId && tenantId.Value != Guid.Empty) return Forbid();

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
