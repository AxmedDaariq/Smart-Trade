using System;
using System.Threading;
using System.Threading.Tasks;
using AnprGate.Application.Commands;

namespace AnprGate.Tests.Application.Commands;

public class IngestRecognitionCommandHandlerTests
{
    private readonly IngestRecognitionCommandHandler _handler = new();

    private static IngestRecognitionCommand BuildCommand(
        Guid? tenantId = null,
        Guid? cameraId = null,
        string plateText = "AB-123-CD",
        double confidence = 0.97,
        DateTime? observedAt = null,
        string providerEventId = "prov-evt-1")
    {
        return new IngestRecognitionCommand
        {
            TenantId = tenantId ?? Guid.NewGuid(),
            CameraId = cameraId ?? Guid.NewGuid(),
            PlateText = plateText,
            Confidence = confidence,
            ObservedAt = observedAt ?? new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            ProviderEventId = providerEventId
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnANonEmptyRecognitionEventId()
    {
        // Arrange
        var command = BuildCommand();

        // Act
        var recognitionId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, recognitionId);
    }

    [Fact]
    public async Task Handle_CalledTwice_ShouldReturnDistinctIds()
    {
        // Arrange: each ingested frame is its own recognition event
        // Act
        var first = await _handler.Handle(BuildCommand(), CancellationToken.None);
        var second = await _handler.Handle(BuildCommand(), CancellationToken.None);

        // Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task Handle_WithLowConfidenceRecognition_ShouldStillAcceptIt()
    {
        // Arrange: confidence triage happens downstream, ingestion itself must not fail
        var command = BuildCommand(confidence: 0.1);

        // Act
        var recognitionId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, recognitionId);
    }

    [Fact]
    public async Task Handle_WithEmptyPlateText_ShouldStillAcceptIt()
    {
        // Arrange: unreadable plates are a valid camera outcome to ingest
        var command = BuildCommand(plateText: string.Empty);

        // Act
        var recognitionId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, recognitionId);
    }

    [Fact]
    public async Task Handle_ShouldNotMutateTheIncomingCommand()
    {
        // Arrange
        var command = BuildCommand();
        var originalTenant = command.TenantId;
        var originalCamera = command.CameraId;
        var originalPlate = command.PlateText;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(originalTenant, command.TenantId);
        Assert.Equal(originalCamera, command.CameraId);
        Assert.Equal(originalPlate, command.PlateText);
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldNotThrow()
    {
        // Arrange: the current handler performs no I/O, so cancellation is a no-op
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var recognitionId = await _handler.Handle(BuildCommand(), cts.Token);

        // Assert
        Assert.NotEqual(Guid.Empty, recognitionId);
    }
}