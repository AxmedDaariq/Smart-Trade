using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GpsTelematics.Application.Commands;
using GpsTelematics.Application.Interfaces;
using GpsTelematics.Domain.Entities;
using Moq;
using Xunit;

namespace GpsTelematics.UnitTests.Services;

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
        saved.Should().NotBeNull();
        positionId.Should().Be(saved!.Id);
        positionId.Should().NotBeEmpty();
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
        saved.Should().NotBeNull();
        saved!.TenantId.Should().Be(command.TenantId);
        saved.GpsDeviceId.Should().Be(command.DeviceId);
        saved.Latitude.Should().Be(command.Latitude);
        saved.Longitude.Should().Be(command.Longitude);
        saved.SpeedKph.Should().Be(command.Speed);
        saved.HeadingDegrees.Should().Be(command.Heading);
        saved.TimestampUtc.Should().Be(command.RecordedAt);
        saved.SequenceNumber.Should().Be(command.SequenceNumber);
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

        // Assert
        positionId.Should().NotBeEmpty();
        saved!.SequenceNumber.Should().Be(7);
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
        receivedToken.Should().Be(cts.Token);
    }

    [Fact]
    public async Task Handle_WithVehicleIdSupplied_ShouldStillIngestThePosition()
    {
        // Arrange
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
        positionId.Should().NotBeEmpty();
        saved.Should().NotBeNull();
        _repository.Verify(r => r.AddAsync(It.IsAny<GpsPosition>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
