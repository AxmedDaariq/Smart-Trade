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
/// Unit tests for <see cref="ShipmentsController"/> using mocked <see cref="IMediator"/>
/// and <see cref="IShippingQueryContext"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class ShipmentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IShippingQueryContext> _queryMock = new();
    private readonly ShipmentsController _controller;

    public ShipmentsControllerTests()
    {
        _controller = new ShipmentsController(_mediatorMock.Object, _queryMock.Object);
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

    // ─── CreateShipment ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateShipment_WithValidClaims_Returns201CreatedWithShipmentId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();

        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateShipmentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var command = new CreateShipmentCommand();

        // Act
        var result = await _controller.CreateShipment(command);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(ShipmentsController.GetShipment));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(expectedId);
    }

    [Fact]
    public async Task CreateShipment_PopulatesTenantIdFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        CreateShipmentCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateShipmentCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as CreateShipmentCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new CreateShipmentCommand();

        // Act
        await _controller.CreateShipment(command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
        captured.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateShipment_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null, subClaim: Guid.NewGuid().ToString());
        var command = new CreateShipmentCommand();

        // Act
        var result = await _controller.CreateShipment(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateShipmentCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateShipment_InvalidGuidTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "not-a-valid-guid", subClaim: Guid.NewGuid().ToString());
        var command = new CreateShipmentCommand();

        // Act
        var result = await _controller.CreateShipment(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── UpdateStatus ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_WithValidClaims_Returns204NoContent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateShipmentStatusCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new UpdateShipmentStatusCommand();

        // Act
        var result = await _controller.UpdateStatus(shipmentId, command);

        // Assert
        result.Should().BeOfType<NoContentResult>()
              .Which.StatusCode.Should().Be(204);

        _mediatorMock.Verify(m => m.Send(It.Is<UpdateShipmentStatusCommand>(c =>
            c.TenantId == tenantId &&
            c.ShipmentId == shipmentId &&
            c.UserId == userId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new UpdateShipmentStatusCommand();

        // Act
        var result = await _controller.UpdateStatus(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── ConfirmCustomsClearance ──────────────────────────────────────────────

    [Fact]
    public async Task ConfirmCustomsClearance_WithValidClaims_Returns204()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmCustomsClearanceCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new ConfirmCustomsClearanceCommand();

        // Act
        var result = await _controller.ConfirmCustomsClearance(shipmentId, command);

        // Assert
        result.Should().BeOfType<NoContentResult>()
              .Which.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task ConfirmCustomsClearance_MissingTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new ConfirmCustomsClearanceCommand();

        // Act
        var result = await _controller.ConfirmCustomsClearance(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }
}
