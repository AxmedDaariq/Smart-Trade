using Identity.Application.Interfaces;
using System;

namespace Identity.Infrastructure;

public class TenantContext : ITenantContext
{
    public Guid? CurrentTenantId { get; private set; }
    public string? CurrentTenantCode { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        CurrentTenantId = tenantId;
    }

    public void SetTenant(string tenantCode)
    {
        CurrentTenantCode = tenantCode;
    }
}
