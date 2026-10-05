using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for the transactional outbox <see cref="OutboxMessage"/>.
/// </summary>
public class OutboxMessageTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var message = new OutboxMessage();
        var after   = DateTime.UtcNow.AddSeconds(1);

        message.Id.Should().NotBe(Guid.Empty);
        message.MessageId.Should().Be(Guid.Empty);
        message.TenantId.Should().BeNull();
        message.EventType.Should().BeEmpty();
        message.AggregateType.Should().BeEmpty();
        message.AggregateId.Should().Be(Guid.Empty);
        message.PayloadJson.Should().BeEmpty();
        message.SchemaVersion.Should().Be("1.0");
        message.CorrelationId.Should().BeEmpty();
        message.CausationId.Should().BeNull();
        message.Status.Should().Be("Pending");
        message.RetryCount.Should().Be(0);
        message.LastAttemptAtUtc.Should().BeNull();
        message.PublishedAtUtc.Should().BeNull();
        message.ErrorMessage.Should().BeNull();
        message.OccurredAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var messageId   = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        var message = new OutboxMessage
        {
            MessageId     = messageId,
            TenantId      = Guid.NewGuid(),
            EventType     = "AuditRecorded",
            AggregateType = "AuditEvent",
            AggregateId   = aggregateId,
            PayloadJson   = "{\"event\":\"created\"}",
            CorrelationId = "corr-1",
            Status        = "Published"
        };

        message.MessageId.Should().Be(messageId);
        message.EventType.Should().Be("AuditRecorded");
        message.AggregateType.Should().Be("AuditEvent");
        message.AggregateId.Should().Be(aggregateId);
        message.PayloadJson.Should().Be("{\"event\":\"created\"}");
        message.CorrelationId.Should().Be("corr-1");
        message.Status.Should().Be("Published");
    }
}
