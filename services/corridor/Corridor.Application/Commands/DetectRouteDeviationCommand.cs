using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Corridor.Domain.Entities;
using Corridor.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Corridor.Application.Commands;

public class DetectRouteDeviationCommand : IRequest<bool>
{
    public Guid TenantId  { get; set; }
    public Guid TripId    { get; set; }
    public double Latitude  { get; set; }
    public double Longitude { get; set; }
}

/// <summary>
/// Application-layer abstraction over the Corridor persistence context.
/// Keeps Application free of any Infrastructure / EF dependency.
/// </summary>
public interface ICorridorDbContext
{
    IQueryable<Trip>         Trips        { get; }
    IQueryable<RouteVersion> RouteVersions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class DetectRouteDeviationCommandHandler : IRequestHandler<DetectRouteDeviationCommand, bool>
{
    private readonly ICorridorDbContext _context;

    public DetectRouteDeviationCommandHandler(ICorridorDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DetectRouteDeviationCommand request, CancellationToken cancellationToken)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.Id == request.TripId && t.TenantId == request.TenantId, cancellationToken);

        if (trip == null) throw new InvalidOperationException("Trip not found.");

        var routeVersion = await _context.RouteVersions
            .FirstOrDefaultAsync(r => r.Id == trip.RouteVersionId, cancellationToken);

        if (routeVersion == null) throw new InvalidOperationException("RouteVersion not found.");

        // Create a PostGIS Point from the incoming GPS coordinates
        var point = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude) { SRID = 4326 };

        // PostGIS distance check (simplified threshold comparison)
        var distance  = routeVersion.Geometry.Distance(point);
        var threshold = 0.005; // approx ~500m depending on projection

        if (distance > threshold)
        {
            // Route Deviation Detected — raise event / create RouteDeviation entity etc.
            return true;
        }

        return false;
    }
}
