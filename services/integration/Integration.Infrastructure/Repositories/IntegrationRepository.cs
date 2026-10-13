using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Integration.Domain.Entities;
using Integration.Domain.Repositories;
using Integration.Infrastructure.Persistence;

namespace Integration.Infrastructure.Repositories;

public class IntegrationRepository : IIntegrationRepository
{
    private readonly IntegrationDbContext _context;

    public IntegrationRepository(IntegrationDbContext context)
    {
        _context = context;
    }

    public async Task<WebhookEndpoint> GetEndpointByPathAsync(string path, Guid tenantId)
    {
        return await _context.WebhookEndpoints.FirstOrDefaultAsync(e => e.Path == path && e.TenantId == tenantId);
    }

    public async Task<Partner> GetPartnerByCodeAsync(string partnerCode, Guid tenantId)
    {
        return await _context.Partners.FirstOrDefaultAsync(p => p.PartnerCode == partnerCode && p.TenantId == tenantId);
    }

    public async Task AddAsync(object entity)
    {
        await _context.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
