using System;
using AnprGate.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AnprGate.UnitTests.Domain;

public class AnprRecognitionEventTests
{
    [Fact]
    public void NewRecognitionEvent_ShouldDefaultToReceivedStatusAndUnmatched()
    {
        // Arrange & Act
        var recognitionEvent = new AnprRecognitionEvent();

        // Assert
        recognitionEvent.Status.Should().Be("Received");
        recognitionEvent.VehicleMatchStatus.Should().Be("Unmatched");
        recognitionEvent.TripMatchStatus.Should().Be("Unmatched");
        recognitionEvent.RecognitionReference.Should().BeEmpty();
        recognitionEvent.NormalizedPlateText.Should().BeEmpty();
        recognitionEvent.Direction.Should().BeEmpty();
        recognitionEvent.Source.Should().BeEmpty();
        recognitionEvent.CorrelationId.Should().BeEmpty();
        recognitionEvent.OriginalPlateText.Should().BeNull();
        recognitionEvent.Confidence.Should().BeNull();
        recognitionEvent.GateId.Should().BeNull();
        recognitionEvent.LaneId.Should().BeNull();
        recognitionEvent.CountryCode.Should().BeNull();
        recognitionEvent.EvidenceReference.Should().BeNull();
        recognitionEvent.SequenceNumber.Should().BeNull();
    }

    [Fact]
    public void NewRecognitionEvent_ShouldHaveEmptyChildCollections()
    {
        // Arrange & Act
        var recognitionEvent = new AnprRecognitionEvent();

        // Assert
        recognitionEvent.PlateObservations.Should().BeEmpty();
        recognitionEvent.VehicleMatches.Should().BeEmpty();
        recognitionEvent.TripMatches.Should().BeEmpty();
        recognitionEvent.Evidences.Should().BeEmpty();
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
        recognitionEvent.Id.Should().NotBeEmpty();
        recognitionEvent.ReceivedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        recognitionEvent.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NewRecognitionEvents_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new AnprRecognitionEvent();
        var second = new AnprRecognitionEvent();

        // Assert
        first.Id.Should().NotBe(second.Id);
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
        recognitionEvent.NormalizedPlateText.Should().Be("AB123CD");
        recognitionEvent.OriginalPlateText.Should().Be("AB-123-CD");
        recognitionEvent.Confidence.Should().Be(0.94);
        recognitionEvent.RecognizedAtUtc.Should().Be(recognizedAt);
        recognitionEvent.Status.Should().Be("Validated");
        recognitionEvent.VehicleMatchStatus.Should().Be("Matched");
    }
}
