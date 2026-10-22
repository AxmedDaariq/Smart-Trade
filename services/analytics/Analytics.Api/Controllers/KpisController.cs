using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace SmartTrade.Analytics.Api.Controllers;

/// <summary>
/// A01: KPI definitions are tenant data. The endpoint is authenticated, requires the
/// <c>analytics.kpi.read</c> permission and always evaluates the tenant taken from the JWT -
/// a tenantId supplied in the query string can only confirm, never widen, the caller's scope.
/// </summary>
[ApiController]
[Route("api/v1/analytics/[controller]")]
[Authorize(Policy = "analytics.kpi.read")]
public class KpisController : ControllerBase
{
    [HttpGet]
    public IActionResult GetKpis([FromQuery] Guid? tenantId, [FromQuery] string category = null)
    {
        if (!TryResolveTenant(tenantId, out var effectiveTenantId)) return Forbid();

        // 1. Fetch KPI definitions scoped to effectiveTenantId
        // 2. Return DTOs
        return Ok(new[]
        {
            new { Id = Guid.NewGuid(), KpiCode = "TRANSIT_TIME", Category = "Logistics", Status = "Active", TenantId = effectiveTenantId }
        });
    }

    [HttpGet("{code}")]
    public IActionResult GetKpiResult(string code, [FromQuery] Guid? tenantId, [FromQuery] DateTimeOffset start, [FromQuery] DateTimeOffset end)
    {
        if (!TryResolveTenant(tenantId, out var effectiveTenantId)) return Forbid();

        // 1. Fetch KPI result from Redis Cache or PostgreSQL, scoped to effectiveTenantId
        return Ok(new
        {
            KpiCode = code,
            TenantId = effectiveTenantId,
            Value = 120.5m,
            Target = 100.0m,
            Status = "Warning",
            DataFreshness = "Fresh"
        });
    }

    /// <summary>
    /// Resolves the tenant from the <c>tenant_id</c> claim and rejects any request that asks for
    /// another tenant's data.
    /// </summary>
    private bool TryResolveTenant(Guid? requestedTenantId, out Guid effectiveTenantId)
    {
        effectiveTenantId = Guid.Empty;

        var claim = User.FindFirst("tenant_id")?.Value;
        if (!Guid.TryParse(claim, out var callerTenantId)) return false;

        if (requestedTenantId.HasValue && requestedTenantId.Value != callerTenantId &&
            requestedTenantId.Value != Guid.Empty)
        {
            // Explicit cross-tenant request from a non-platform caller.
            return false;
        }

        effectiveTenantId = callerTenantId;
        return true;
    }
}
