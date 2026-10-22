using System;
using AnprGate.Domain.Entities;

namespace AnprGate.Tests.Domain.Entities;

public class AnprRecognitionEventTests
{
    [Fact]
    public void NewRecognitionEvent_ShouldDefaultToReceivedStatusAndUnmatched()
    {
        // Arrange & Act
        var recognitionEvent = new AnprRecognitionEvent();

        // Assert
        Assert.Equal("Received", recognitionEvent.Status);
        Assert.Equal("Unmatched", recognitionEvent.VehicleMatchStatus);
        Assert.Equal("Unmatched", recognitionEvent.TripMatchStatus);
        Assert.Equal(string.Empty, recognitionEvent.RecognitionReference);
        Assert.Equal(string.Empty, recognitionEvent.NormalizedPlateText);
        Assert.Equal(string.Empty, recognitionEvent.Direction);
        Assert.Equal(string.Empty, recognitionEvent.Source);
        Assert.Equal(string.Empty, recognitionEvent.CorrelationId);
        Assert.Null(recognitionEvent.OriginalPlateText);
        Assert.Null(recognitionEvent.Confidence);
        Assert.Null(recognitionEvent.GateId);
        Assert.Null(recognitionEvent.LaneId);
        Assert.Null(recognitionEvent.CountryCode);
        Assert.Null(recognitionEvent.EvidenceReference);
        Assert.Null(recognitionEvent.SequenceNumber);
    }

    [Fact]
    public void NewRecognitionEvent_ShouldHaveEmptyChildCollections()
    {
        // Arrange & Act
        var recognitionEvent = new AnprRecognitionEvent();

        // Assert
        Assert.Empty(recognitionEvent.PlateObservations);
        Assert.Empty(recognitionEvent.VehicleMatches);
        Assert.Empty(recognitionEvent.TripMatches);
        Assert.Empty(recognitionEvent.Evidences);
    }

    [Fact]
    public void NewRecognitionEvent_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var recognitionEvent = new AnprRecognitionEvent();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, recognitionEvent.Id);
        Assert.InRange(recognitionEvent.ReceivedAtUtc, before, after);
        Assert.InRange(recognitionEvent.CreatedAtUtc, before, after);
    }

    [Fact]
    public void NewRecognitionEvents_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new AnprRecognitionEvent();
        var second = new AnprRecognitionEvent();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void RecognitionEventProperties_ShouldBeSettable()
    {
        // Arrange
        var recognitionEvent = new AnprRecognitionEvent();
        var recognizedAt = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        recognitionEvent.NormalizedPlateText = "AB123CD";
        recognitionEvent.OriginalPlateText = "AB-123-CD";
        recognitionEvent.Confidence = 0.94;
        recognitionEvent.RecognizedAtUtc = recognizedAt;
        recognitionEvent.Status = "Validated";
        recognitionEvent.VehicleMatchStatus = "Matched";

        // Assert
        Assert.Equal("AB123CD", recognitionEvent.NormalizedPlateText);
        Assert.Equal("AB-123-CD", recognitionEvent.OriginalPlateText);
        Assert.Equal(0.94, recognitionEvent.Confidence);
        Assert.Equal(recognizedAt, recognitionEvent.RecognizedAtUtc);
        Assert.Equal("Validated", recognitionEvent.Status);
        Assert.Equal("Matched", recognitionEvent.VehicleMatchStatus);
    }
}