using Microsoft.AspNetCore.Http;
using Identity.Application.Interfaces;

namespace Identity.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // Example: Resolve tenant from header or JWT Claims
        if (context.Request.Headers.TryGetValue("X-Tenant-Code", out var tenantCodeHeader))
        {
            var code = tenantCodeHeader.FirstOrDefault();
            if (!string.IsNullOrEmpty(code))
            {
                // In a real implementation, you would validate this code exists
                // and assign it to the scoped ITenantContext.
                // tenantContext.SetTenant(code);
            }
        }
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            // Extract from JWT "tenant_id" claim
            var tenantClaim = context.User.FindFirst("tenant_id");
            if (tenantClaim != null)
            {
                // tenantContext.SetTenant(Guid.Parse(tenantClaim.Value));
            }
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}
