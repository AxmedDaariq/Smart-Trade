using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditArchive"/>.
/// </summary>
public class AuditArchiveTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var archive = new AuditArchive();
        var after  = DateTime.UtcNow.AddSeconds(1);

        archive.Id.Should().NotBe(Guid.Empty);
        archive.TenantId.Should().BeNull();
        archive.ArchiveReference.Should().BeEmpty();
        archive.PeriodStartUtc.Should().Be(default(DateTime));
        archive.PeriodEndUtc.Should().Be(default(DateTime));
        archive.RecordCount.Should().Be(0);
        archive.StorageReference.Should().BeEmpty();
        archive.ContentHash.Should().BeEmpty();
        archive.EncryptionStatus.Should().Be("Encrypted");
        archive.VerificationStatus.Should().Be("Pending");
        archive.ArchivedAtUtc.Should().BeNull();
        archive.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var start = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end   = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var archive = new AuditArchive
        {
            TenantId           = Guid.NewGuid(),
            ArchiveReference   = "ARC-2025",
            PeriodStartUtc     = start,
            PeriodEndUtc       = end,
            RecordCount        = 1_000_000,
            StorageReference   = "s3://audit-archive/2025",
            ContentHash        = "sha256-abc",
            EncryptionStatus   = "Encrypted",
            VerificationStatus = "Verified"
        };

        archive.ArchiveReference.Should().Be("ARC-2025");
        archive.PeriodStartUtc.Should().Be(start);
        archive.PeriodEndUtc.Should().Be(end);
        archive.RecordCount.Should().Be(1_000_000);
        archive.StorageReference.Should().Be("s3://audit-archive/2025");
        archive.ContentHash.Should().Be("sha256-abc");
        archive.VerificationStatus.Should().Be("Verified");
    }
}
