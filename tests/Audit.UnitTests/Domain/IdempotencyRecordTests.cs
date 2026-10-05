using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="IdempotencyRecord"/>.
/// </summary>
public class IdempotencyRecordTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var record = new IdempotencyRecord();
        var after  = DateTime.UtcNow.AddSeconds(1);

        record.Id.Should().NotBe(Guid.Empty);
        record.TenantId.Should().BeNull();
        record.IdempotencyKey.Should().BeEmpty();
        record.Operation.Should().BeEmpty();
        record.MessageId.Should().BeNull();
        record.RequestHash.Should().BeEmpty();
        record.ResponseStatus.Should().BeNull();

        // ExpiresAtUtc has no initializer and must be supplied by the caller.
        record.ExpiresAtUtc.Should().Be(default(DateTime));
        record.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var messageId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(1);

        var record = new IdempotencyRecord
        {
            TenantId       = Guid.NewGuid(),
            IdempotencyKey = "idem-key-1",
            Operation      = "RecordAuditEvent",
            MessageId      = messageId,
            RequestHash    = "sha256-request",
            ResponseStatus = 201,
            ExpiresAtUtc   = expiresAt
        };

        record.IdempotencyKey.Should().Be("idem-key-1");
        record.Operation.Should().Be("RecordAuditEvent");
        record.MessageId.Should().Be(messageId);
        record.RequestHash.Should().Be("sha256-request");
        record.ResponseStatus.Should().Be(201);
        record.ExpiresAtUtc.Should().Be(expiresAt);
    }
}
