using Xunit;
using FluentAssertions;
using Audit.Domain.Entities;

namespace Audit.UnitTests.Domain;

public class AuditEventTests
{
    [Fact]
    public void AuditEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var auditEvent = new AuditEvent();

        // Assert
        auditEvent.Id.Should().NotBeEmpty();
        auditEvent.Severity.Should().Be("Informational");
        auditEvent.DataClassification.Should().Be("Internal");
        auditEvent.SchemaVersion.Should().Be("1.0");
        auditEvent.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AuditEvent_ShouldAllowPropertyUpdates()
    {
        // Arrange
        var auditEvent = new AuditEvent();
        var eventType = "UserLogin";
        var action = "LoginAttempt";
        var outcome = "Success";

        // Act
        auditEvent.EventType = eventType;
        auditEvent.Action = action;
        auditEvent.Outcome = outcome;

        // Assert
        auditEvent.EventType.Should().Be(eventType);
        auditEvent.Action.Should().Be(action);
        auditEvent.Outcome.Should().Be(outcome);
    }
}
