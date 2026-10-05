using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditIntegrityRecord"/> (cryptographic hash-chain blocks).
/// </summary>
public class AuditIntegrityRecordTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var record = new AuditIntegrityRecord();
        var after  = DateTime.UtcNow.AddSeconds(1);

        record.Id.Should().NotBe(Guid.Empty);
        record.TenantId.Should().BeNull();
        record.PartitionReference.Should().BeNull();
        record.FirstAuditEventId.Should().Be(Guid.Empty);
        record.LastAuditEventId.Should().Be(Guid.Empty);
        record.EventCount.Should().Be(0);
        record.FirstHash.Should().BeEmpty();
        record.LastHash.Should().BeEmpty();
        record.VerificationStatus.Should().Be("NotVerified");
        record.VerificationError.Should().BeNull();
        record.CalculatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var firstId = Guid.NewGuid();
        var lastId  = Guid.NewGuid();

        var record = new AuditIntegrityRecord
        {
            TenantId           = Guid.NewGuid(),
            PartitionReference = "2026-01",
            FirstAuditEventId  = firstId,
            LastAuditEventId   = lastId,
            EventCount         = 1500,
            FirstHash          = "AAA",
            LastHash           = "ZZZ",
            VerificationStatus = "Valid",
            VerificationError  = null
        };

        record.PartitionReference.Should().Be("2026-01");
        record.FirstAuditEventId.Should().Be(firstId);
        record.LastAuditEventId.Should().Be(lastId);
        record.EventCount.Should().Be(1500);
        record.FirstHash.Should().Be("AAA");
        record.LastHash.Should().Be("ZZZ");
        record.VerificationStatus.Should().Be("Valid");
    }
}
