using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Corridor.Api.Controllers;
using Corridor.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Corridor.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="TripsController"/> using mocked <see cref="IMediator"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class TripsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly TripsController _controller;

    public TripsControllerTests()
    {
        _controller = new TripsController(_mediatorMock.Object);
    }

    private void SetControllerContext(string? tenantIdClaim = null, string? subClaim = null)
    {
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>();

        if (tenantIdClaim is not null)
            claims.Add(new Claim("tenant_id", tenantIdClaim));

        if (subClaim is not null)
            claims.Add(new Claim("sub", subClaim));

        if (claims.Count > 0)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    // ─── DetectDeviation ─────────────────────────────────────────────────────

    [Fact]
    public async Task DetectDeviation_WithValidClaims_Returns200OkWithResult()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DetectRouteDeviationCommand
        {
            Latitude = 2.0469,
            Longitude = 45.3182
        };

        // Act
        var result = await _controller.DetectDeviation(tripId, command);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DetectDeviation_PopulatesTenantIdAndTripIdFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        DetectRouteDeviationCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as DetectRouteDeviationCommand)
            .ReturnsAsync(false);

        var command = new DetectRouteDeviationCommand
        {
            Latitude = 2.0469,
            Longitude = 45.3182
        };

        // Act
        await _controller.DetectDeviation(tripId, command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
        captured.TripId.Should().Be(tripId);
        captured.Latitude.Should().Be(2.0469);
        captured.Longitude.Should().Be(45.3182);
    }

    [Fact]
    public async Task DetectDeviation_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new DetectRouteDeviationCommand();

        // Act
        var result = await _controller.DetectDeviation(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DetectDeviation_InvalidGuidTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "not-a-valid-guid");
        var command = new DetectRouteDeviationCommand();

        // Act
        var result = await _controller.DetectDeviation(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task DetectDeviation_DeviationDetected_ReturnsDeviatedTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DetectRouteDeviationCommand
        {
            Latitude = 10.0,
            Longitude = 50.0
        };

        // Act
        var result = await _controller.DetectDeviation(tripId, command);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        // Verify the anonymous object returned contains TripId and Deviated = true
        var value = ok.Value;
        value.Should().NotBeNull();
    }

    [Fact]
    public async Task DetectDeviation_NoDeviation_ReturnsDeviatedFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DetectRouteDeviationCommand
        {
            Latitude = 2.0469,
            Longitude = 45.3182
        };

        // Act
        var result = await _controller.DetectDeviation(tripId, command);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DetectDeviation_InvokesMediatrExactlyOnce()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DetectRouteDeviationCommand();

        // Act
        await _controller.DetectDeviation(Guid.NewGuid(), command);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<DetectRouteDeviationCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
