using System;
using System.Threading.Tasks;
using Integration.Domain.Entities;

namespace Integration.Domain.Repositories;

public interface IIntegrationRepository
{
    Task<WebhookEndpoint> GetEndpointByPathAsync(string path, Guid tenantId);
    Task<Partner> GetPartnerByCodeAsync(string partnerCode, Guid tenantId);
    Task AddAsync(object entity);
    Task SaveChangesAsync();
}
