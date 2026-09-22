namespace Identity.Application.Interfaces;

public interface ITenantContext
{
    Guid? CurrentTenantId { get; }
    string? CurrentTenantCode { get; }
}
