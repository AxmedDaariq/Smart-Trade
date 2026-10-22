using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Analytics.Api.Controllers;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Analytics.UnitTests.Controllers;

public class KpiControllerTests : IDisposable
{
    private readonly AnalyticsDbContext _context;
    private readonly KpiController _controller;

    public KpiControllerTests()
    {
        var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseInMemoryDatabase(databaseName: $"AnalyticsTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AnalyticsDbContext(options);
        _controller = new KpiController(_context);
    }

    private void SetupUser(Guid tenantId)
    {
        var claims = new List<Claim>
        {
            new Claim("tenant_id", tenantId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    [Fact]
    public async Task GetTransitTimeKpi_ShouldReturnForbid_WhenTenantIdClaimIsMissing()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        // Act
        var result = await _controller.GetTransitTimeKpi(null);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetTransitTimeKpi_ShouldReturnForbid_WhenRequestedTenantDiffersFromUserTenant()
    {
        // Arrange
        var userTenantId = Guid.NewGuid();
        var requestedTenantId = Guid.NewGuid();
        SetupUser(userTenantId);

        // Act
        var result = await _controller.GetTransitTimeKpi(requestedTenantId);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetTransitTimeKpi_ShouldReturnOk_WhenTenantIdIsSameAsUserTenant()
    {
        // Arrange
        var userTenantId = Guid.NewGuid();
        SetupUser(userTenantId);

        var milestone = new FactTripMilestone(
            tenantId: userTenantId,
            tripId: Guid.NewGuid(),
            vehicleId: Guid.NewGuid(),
            corridorId: Guid.NewGuid(),
            milestoneType: "PortExit",
            locationName: "Port of Djibouti",
            occurredAt: DateTime.UtcNow,
            expectedDuration: TimeSpan.FromHours(4),
            actualDuration: TimeSpan.FromHours(4.5),
            delayDuration: TimeSpan.FromMinutes(30),
            sourceEventId: Guid.NewGuid());

        _context.FactTripMilestones.Add(milestone);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTransitTimeKpi(userTenantId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
