using System.Threading;
using System.Threading.Tasks;
using GpsTelematics.Application.Interfaces;
using GpsTelematics.Domain.Entities;
using GpsTelematics.Infrastructure.Persistence;

namespace GpsTelematics.Infrastructure.Repositories;

public class TelemetryIngestionRepository : ITelemetryIngestionRepository
{
    private readonly GpsDbContext _context;

    public TelemetryIngestionRepository(GpsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(GpsPosition position, CancellationToken cancellationToken = default)
    {
        await _context.GpsPositions.AddAsync(position, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
