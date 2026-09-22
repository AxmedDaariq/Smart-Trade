using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GpsTelematics.Domain.Entities;
using GpsTelematics.Infrastructure.Persistence;

namespace GpsTelematics.Application.Commands;

public class IngestTelemetryCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public DateTime RecordedAt { get; set; }
    public long SequenceNumber { get; set; }
    // VehicleId is resolved during processing if device is actively assigned
    public Guid? VehicleId { get; set; } 
}

public class IngestTelemetryCommandHandler : IRequestHandler<IngestTelemetryCommand, Guid>
{
    private readonly GpsDbContext _context;

    public IngestTelemetryCommandHandler(GpsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(IngestTelemetryCommand request, CancellationToken cancellationToken)
    {
        var position = new GpsPosition(
            request.TenantId,
            request.DeviceId,
            request.VehicleId,
            request.Latitude,
            request.Longitude,
            request.Speed,
            request.Heading,
            request.RecordedAt,
            request.SequenceNumber
        );

        _context.Positions.Add(position);
        
        // Logic to update LatestLocationCache (Redis) would go here
        
        await _context.SaveChangesAsync(cancellationToken);

        return position.Id;
    }
}
