using System;
using System.Linq;
using System.Threading.Tasks;
using Audit.Domain.Entities;
using Audit.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audit.UnitTests.Services;

public class AuditDbContextTests : IDisposable
{
    private readonly AuditDbContext _context;

    public AuditDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseInMemoryDatabase(databaseName: $"AuditTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuditDbContext(options);
    }

    [Fact]
    public async Task AddAuditEvent_ShouldPersistEvent_AndBeRetrievableByTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();

        var event1 = new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventType = "Customs.Declaration.Submitted",
            Action = "Submit",
            Outcome = "Success",
            Severity = "Informational",
            DataClassification = "Internal"
        };

        var event2 = new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = otherTenantId,
            EventType = "Fleet.Dispatch.Created",
            Action = "Create",
            Outcome = "Success",
            Severity = "Informational",
            DataClassification = "Internal"
        };

        // Act
        _context.AuditEvents.AddRange(event1, event2);
        await _context.SaveChangesAsync();

        var tenantEvents = await _context.AuditEvents
            .Where(e => e.TenantId == tenantId)
            .ToListAsync();

        // Assert
        tenantEvents.Should().HaveCount(1);
        tenantEvents[0].EventType.Should().Be("Customs.Declaration.Submitted");
        tenantEvents[0].Outcome.Should().Be("Success");
    }

    [Fact]
    public async Task AddSecurityEvent_ShouldPersist_WithRiskLevelAndCategory()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventType = "Security.ThreatDetected"
        };
        _context.AuditEvents.Add(auditEvent);
        await _context.SaveChangesAsync();

        var secEvent = new AuditSecurityEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AuditEventId = auditEvent.Id,
            SecurityEventType = "UnauthorizedAccess",
            RiskLevel = "High",
            DetectionReason = "Brute force attempt detected on gateway",
            IpAddress = "192.168.1.100"
        };

        // Act
        _context.SecurityEvents.Add(secEvent);
        await _context.SaveChangesAsync();

        var retrieved = await _context.SecurityEvents
            .FirstOrDefaultAsync(s => s.Id == secEvent.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.SecurityEventType.Should().Be("UnauthorizedAccess");
        retrieved.RiskLevel.Should().Be("High");
        retrieved.IpAddress.Should().Be("192.168.1.100");
    }

    [Fact]
    public async Task AddAlert_ShouldPersistAndAllowResolutionStatusUpdate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var alert = new AuditAlert
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AlertReference = "ALT-2026-001",
            AlertType = "SuspiciousAccess",
            Severity = "Critical",
            Status = "Open"
        };

        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        // Act
        alert.Status = "Acknowledged";
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();

        var updated = await _context.Alerts.FindAsync(alert.Id);

        // Assert
        updated.Should().NotBeNull();
        updated!.Status.Should().Be("Acknowledged");
    }

    [Fact]
    public async Task IdempotencyRecord_ShouldPersist_KeyAndOperation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var record = new IdempotencyRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            IdempotencyKey = "key-abc-123",
            Operation = "ProcessAuditEvent",
            ResponseStatus = 200
        };

        // Act
        _context.IdempotencyRecords.Add(record);
        await _context.SaveChangesAsync();

        var retrieved = await _context.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.IdempotencyKey == "key-abc-123");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.ResponseStatus.Should().Be(200);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
