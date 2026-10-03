using System;
using System.Threading;
using System.Threading.Tasks;
using AisVessel.Domain.Entities;

namespace AisVessel.Application.Interfaces;

/// <summary>
/// Read/write access for the berth allocation workflow. Implemented in
/// Infrastructure so the Application layer never depends on Entity Framework
/// or a concrete DbContext.
/// </summary>
public interface IBerthAllocationRepository
{
    Task<PortCall?> GetPortCallAsync(Guid portCallId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Berth?> GetBerthAsync(Guid berthId, Guid tenantId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
