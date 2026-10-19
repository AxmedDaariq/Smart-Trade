using System;
using System.Threading.Tasks;
using Audit.Domain.Entities;
using Audit.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Xunit;

namespace Audit.IntegrationTests;

public class AuditIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _fixture;

    public AuditIntegrationTests(ContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AuditDbContext_CanConnect_AndPersistAuditEvent()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseNpgsql(_fixture.GetPostgresConnectionString())
            .Options;

        await using var db = new AuditDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var canConnect = await db.Database.CanConnectAsync();
        canConnect.Should().BeTrue();

        var auditEvent = new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            EventType = "Audit.IntegrationTest.Created",
            Action = "Create",
            Outcome = "Success",
            Severity = "Informational",
            DataClassification = "Internal"
        };

        // Act
        db.AuditEvents.Add(auditEvent);
        await db.SaveChangesAsync();

        // Assert
        var saved = await db.AuditEvents.FirstOrDefaultAsync(e => e.Id == auditEvent.Id);
        saved.Should().NotBeNull();
        saved!.EventType.Should().Be("Audit.IntegrationTest.Created");
    }
}
