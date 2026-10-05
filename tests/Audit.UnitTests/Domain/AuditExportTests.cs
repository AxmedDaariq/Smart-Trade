using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditExport"/>.
/// </summary>
public class AuditExportTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var export = new AuditExport();
        var after  = DateTime.UtcNow.AddSeconds(1);

        export.Id.Should().NotBe(Guid.Empty);
        export.TenantId.Should().Be(Guid.Empty); // non-nullable tenant key
        export.ExportReference.Should().BeEmpty();
        export.RequestedBy.Should().Be(Guid.Empty);
        export.Purpose.Should().BeEmpty();
        export.FilterJson.Should().BeNull();
        export.Format.Should().Be("CSV");
        export.Status.Should().Be("Pending");
        export.RecordCount.Should().BeNull();
        export.StorageReference.Should().BeNull();
        export.ContentHash.Should().BeNull();
        export.CompletedAtUtc.Should().BeNull();
        export.ExpiresAtUtc.Should().BeNull();
        export.RequestedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var tenantId    = Guid.NewGuid();
        var requestedBy = Guid.NewGuid();
        var completedAt = DateTime.UtcNow;

        var export = new AuditExport
        {
            TenantId         = tenantId,
            ExportReference  = "EXP-2026-0001",
            RequestedBy      = requestedBy,
            Purpose          = "Regulatory submission",
            FilterJson       = "{\"category\":\"Customs\"}",
            Format           = "JSON",
            Status           = "Completed",
            RecordCount      = 512,
            StorageReference = "s3://audit-exports/exp-2026-0001",
            ContentHash      = "sha256-def",
            CompletedAtUtc   = completedAt
        };

        export.TenantId.Should().Be(tenantId);
        export.ExportReference.Should().Be("EXP-2026-0001");
        export.RequestedBy.Should().Be(requestedBy);
        export.Purpose.Should().Be("Regulatory submission");
        export.FilterJson.Should().Be("{\"category\":\"Customs\"}");
        export.Format.Should().Be("JSON");
        export.Status.Should().Be("Completed");
        export.RecordCount.Should().Be(512);
        export.StorageReference.Should().Be("s3://audit-exports/exp-2026-0001");
        export.ContentHash.Should().Be("sha256-def");
        export.CompletedAtUtc.Should().Be(completedAt);
    }
}

/// <summary>
/// Unit tests for <see cref="AuditExportItem"/>.
/// </summary>
public class AuditExportItemTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var item   = new AuditExportItem();
        var after  = DateTime.UtcNow.AddSeconds(1);

        item.Id.Should().NotBe(Guid.Empty);
        item.TenantId.Should().Be(Guid.Empty); // non-nullable tenant key
        item.ExportId.Should().Be(Guid.Empty);
        item.AuditEventId.Should().Be(Guid.Empty);
        item.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var item = new AuditExportItem();
        item.Export.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var exportId = Guid.NewGuid();
        var eventId  = Guid.NewGuid();

        var item = new AuditExportItem
        {
            TenantId     = Guid.NewGuid(),
            ExportId     = exportId,
            AuditEventId = eventId
        };

        item.ExportId.Should().Be(exportId);
        item.AuditEventId.Should().Be(eventId);
    }
}
