using System;
using System.Threading.Tasks;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Domain.Repositories;

public interface IGpsRepository
{
    Task<GpsPosition> GetPositionByIdAsync(Guid id, Guid tenantId);
    Task AddPositionAsync(GpsPosition position);
    Task SaveChangesAsync();
}
