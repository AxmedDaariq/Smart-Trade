using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GpsTelematics.Application.Interfaces;
using GpsTelematics.Domain.Entities;

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
    private readonly ITelemetryIngestionRepository _repository;

    public IngestTelemetryCommandHandler(ITelemetryIngestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(IngestTelemetryCommand request, CancellationToken cancellationToken)
    {
        var position = new GpsPosition
        {
            TenantId = request.TenantId,
            GpsDeviceId = request.DeviceId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            SpeedKph = request.Speed,
            HeadingDegrees = request.Heading,
            TimestampUtc = request.RecordedAt,
            SequenceNumber = request.SequenceNumber
            // request.VehicleId is intentionally not mapped yet: the vehicle is only
            // resolved once the device's active assignment is known, and the position
            // row stores the resolved vehicle reference instead.
        };

        // Logic to update LatestLocationCache (Redis) would go here

        await _repository.AddAsync(position, cancellationToken);

        return position.Id;
    }
}
