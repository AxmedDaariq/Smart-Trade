using System.Threading;
using System.Threading.Tasks;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Application.Interfaces;

/// <summary>
/// Persists ingested telemetry positions. Implemented in Infrastructure so the
/// Application layer never depends on Entity Framework or a concrete DbContext.
/// </summary>
public interface ITelemetryIngestionRepository
{
    Task AddAsync(GpsPosition position, CancellationToken cancellationToken = default);
}
