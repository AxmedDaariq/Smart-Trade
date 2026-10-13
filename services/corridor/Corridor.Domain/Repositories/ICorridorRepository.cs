using System;
using System.Threading.Tasks;
using Corridor.Domain.Entities;

namespace Corridor.Domain.Repositories;

public interface ICorridorRepository
{
    Task<Trip> GetTripByIdAsync(Guid id, Guid tenantId);
    Task<RouteVersion> GetRouteVersionByIdAsync(Guid id);
    Task AddAsync(object entity);
    Task UpdateAsync(object entity);
    Task SaveChangesAsync();
}
