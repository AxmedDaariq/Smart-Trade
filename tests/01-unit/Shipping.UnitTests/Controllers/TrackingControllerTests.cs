using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shipping.Api.Controllers;
using Shipping.Application.Commands;
using Shipping.Infrastructure.Persistence;
using Xunit;

namespace Shipping.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="TrackingController"/> using mocked <see cref="IMediator"/>
/// and <see cref="IShippingQueryContext"/>.
/// Tests recording tracking events in isolation.
/// </summary>
public sealed class TrackingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IShippingQueryContext> _queryMock = new();
    private readonly TrackingController _controller;

    public TrackingControllerTests()
    {
        _controller = new TrackingController(_mediatorMock.Object, _queryMock.Object);
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

    // ─── RecordTrackingEvent ─────────────────────────────────────────────────

    [Fact]
    public async Task RecordTrackingEvent_WithValidClaims_Returns201Created()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();

        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RecordTrackingEventCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEventId);

        var command = new RecordTrackingEventCommand();

        // Act
        var result = await _controller.RecordTrackingEvent(shipmentId, command);

        // Assert
        var created = result.Should().BeOfType<CreatedResult>().Subject;
        created.StatusCode.Should().Be(201);

        _mediatorMock.Verify(m => m.Send(It.Is<RecordTrackingEventCommand>(c =>
            c.TenantId == tenantId &&
            c.ShipmentId == shipmentId &&
            c.UserId == userId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecordTrackingEvent_PopulatesCommandFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();

        SetControllerContext(tenantId.ToString(), userId.ToString());

        RecordTrackingEventCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RecordTrackingEventCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as RecordTrackingEventCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new RecordTrackingEventCommand();

        // Act
        await _controller.RecordTrackingEvent(shipmentId, command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
        captured.ShipmentId.Should().Be(shipmentId);
        captured.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task RecordTrackingEvent_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null, subClaim: Guid.NewGuid().ToString());
        var command = new RecordTrackingEventCommand();

        // Act
        var result = await _controller.RecordTrackingEvent(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<RecordTrackingEventCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RecordTrackingEvent_InvalidGuidTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "not-a-guid");

        // Act
        var result = await _controller.RecordTrackingEvent(Guid.NewGuid(), new RecordTrackingEventCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }
}
