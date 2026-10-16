using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTrade.Analytics.Api.Controllers;
using Xunit;

namespace Analytics.UnitTests.Controllers;

public class KpisControllerTests
{
    private readonly KpisController _controller;

    public KpisControllerTests()
    {
        _controller = new KpisController();
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
    public void GetKpis_ShouldReturnForbid_WhenTenantIdClaimIsMissing()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Act
        var result = _controller.GetKpis(null);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public void GetKpis_ShouldReturnForbid_WhenRequestedTenantDiffersFromCallerTenant()
    {
        // Arrange
        var callerTenantId = Guid.NewGuid();
        var foreignTenantId = Guid.NewGuid();
        SetupUser(callerTenantId);

        // Act
        var result = _controller.GetKpis(foreignTenantId);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public void GetKpis_ShouldReturnOk_WhenRequestedTenantMatchesCallerTenant()
    {
        // Arrange
        var callerTenantId = Guid.NewGuid();
        SetupUser(callerTenantId);

        // Act
        var result = _controller.GetKpis(callerTenantId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetKpiResult_ShouldReturnOk_WithCalculatedKpi_WhenAuthorized()
    {
        // Arrange
        var callerTenantId = Guid.NewGuid();
        SetupUser(callerTenantId);

        // Act
        var result = _controller.GetKpiResult("TRANSIT_TIME", callerTenantId, DateTimeOffset.UtcNow.AddDays(-7), DateTimeOffset.UtcNow);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetKpiResult_ShouldReturnForbid_WhenTenantMismatches()
    {
        // Arrange
        var callerTenantId = Guid.NewGuid();
        var requestedTenantId = Guid.NewGuid();
        SetupUser(callerTenantId);

        // Act
        var result = _controller.GetKpiResult("TRANSIT_TIME", requestedTenantId, DateTimeOffset.UtcNow.AddDays(-7), DateTimeOffset.UtcNow);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }
}
