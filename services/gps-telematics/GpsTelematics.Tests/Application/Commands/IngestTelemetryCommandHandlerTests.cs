using System;
using System.Threading;
using System.Threading.Tasks;
using GpsTelematics.Application.Commands;
using GpsTelematics.Application.Interfaces;
using GpsTelematics.Domain.Entities;
using Moq;

namespace GpsTelematics.Tests.Application.Commands;

public class IngestTelemetryCommandHandlerTests
{
    private readonly Mock<ITelemetryIngestionRepository> _repository = new();
    private readonly IngestTelemetryCommandHandler _handler;

    public IngestTelemetryCommandHandlerTests()
    {
        _handler = new IngestTelemetryCommandHandler(_repository.Object);
    }

    private static IngestTelemetryCommand BuildCommand(
        Guid? tenantId = null,
        Guid? deviceId = null,
        double latitude = 11.5919,
        double longitude = 43.1456,
        double speed = 42.5,
        double heading = 187.3,
        DateTime? recordedAt = null,
        long sequenceNumber = 1001)
    {
        return new IngestTelemetryCommand
        {
            TenantId = tenantId ?? Guid.NewGuid(),
            DeviceId = deviceId ?? Guid.NewGuid(),
            Latitude = latitude,
            Longitude = longitude,
            Speed = speed,
            Heading = heading,
            RecordedAt = recordedAt ?? new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            SequenceNumber = sequenceNumber
        };
    }

    private GpsPosition? CapturePersistedPosition()
    {
        GpsPosition? saved = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((position, _) => saved = position)
            .Returns(Task.CompletedTask);
        return saved;
    }

    [Fact]
    public async Task Handle_ShouldPersistPosition_Once_AndReturnItsId()
    {
        // Arrange
        GpsPosition? saved = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((position, _) => saved = position)
            .Returns(Task.CompletedTask);
        var command = BuildCommand();

        // Act
        var positionId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal(saved!.Id, positionId);
        Assert.NotEqual(Guid.Empty, positionId);
        _repository.Verify(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandFieldsOntoPosition()
    {
        // Arrange
        GpsPosition? saved = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((position, _) => saved = position)
            .Returns(Task.CompletedTask);
        var command = BuildCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal(command.TenantId, saved!.TenantId);
        Assert.Equal(command.DeviceId, saved.GpsDeviceId);
        Assert.Equal(command.Latitude, saved.Latitude);
        Assert.Equal(command.Longitude, saved.Longitude);
        Assert.Equal(command.Speed, saved.SpeedKph);
        Assert.Equal(command.Heading, saved.HeadingDegrees);
        Assert.Equal(command.RecordedAt, saved.TimestampUtc);
        Assert.Equal(command.SequenceNumber, saved.SequenceNumber);
    }

    [Fact]
    public async Task Handle_ShouldAcceptOutOfOrderSequenceNumbers()
    {
        // Arrange: a device may re-send older frames after a network outage
        GpsPosition? saved = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((position, _) => saved = position)
            .Returns(Task.CompletedTask);
        var command = BuildCommand(sequenceNumber: 7);

        // Act
        var positionId = await _handler.Handle(command, CancellationToken.None);

        // Assert: deduplication/validation happens downstream, ingestion itself succeeds
        Assert.NotEqual(Guid.Empty, positionId);
        Assert.Equal(7, saved!.SequenceNumber);
    }

    [Fact]
    public async Task Handle_ShouldForwardTheCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        CancellationToken receivedToken = CancellationToken.None;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((_, token) => receivedToken = token)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(BuildCommand(), cts.Token);

        // Assert
        Assert.Equal(cts.Token, receivedToken);
    }

    [Fact]
    public async Task Handle_WithVehicleIdSupplied_ShouldStillIngestThePosition()
    {
        // Arrange: vehicle resolution is deferred, so an unassigned frame must not fail
        GpsPosition? saved = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()))
            .Callback<GpsPosition, CancellationToken>((position, _) => saved = position)
            .Returns(Task.CompletedTask);
        var command = BuildCommand();
        command.VehicleId = Guid.NewGuid();

        // Act
        var positionId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, positionId);
        Assert.NotNull(saved);
        _repository.Verify(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
