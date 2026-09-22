using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Corridor.Domain.Entities;
using Corridor.Domain.Enums;
using Corridor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corridor.Application.Commands;

public class DetectRouteDeviationCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public Guid TripId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class DetectRouteDeviationCommandHandler : IRequestHandler<DetectRouteDeviationCommand, bool>
{
    private readonly CorridorDbContext _context;

    public DetectRouteDeviationCommandHandler(CorridorDbContext context)
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
        // Note: In real projection ST_Distance with geography types provides meters.
        var distance = routeVersion.Geometry.Distance(point);
        var threshold = 0.005; // approx meters depending on projection

        if (distance > threshold)
        {
            // Route Deviation Detected
            // Publish event, create TripDeviation entity, etc.
            return true;
        }

        return false;
    }
}
