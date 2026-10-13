using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AisVessel.Domain.Entities;

namespace AisVessel.Domain.Repositories;

public interface IAisVesselRepository
{
    Task<Vessel> GetByIdAsync(Guid id, Guid tenantId);
    Task<PortCall> GetPortCallByIdAsync(Guid id, Guid tenantId);
    Task<bool> BerthExistsAsync(Guid berthId, Guid tenantId);
    Task AddAsync(object entity);
    Task UpdateAsync(object entity);
    Task SaveChangesAsync();
}
