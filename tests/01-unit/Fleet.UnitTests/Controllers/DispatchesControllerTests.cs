using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Controllers;
using Fleet.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fleet.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="DispatchesController"/> using mocked <see cref="IMediator"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class DispatchesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly DispatchesController _controller;

    public DispatchesControllerTests()
    {
        _controller = new DispatchesController(_mediatorMock.Object);
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

    // ─── CreateDispatch ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateDispatch_WithValidClaims_Returns201CreatedWithDispatchId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        var expectedId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDispatchCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var command = new CreateDispatchCommand
        {
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.CreateDispatch(command);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(DispatchesController.GetDispatch));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(expectedId);
    }

    [Fact]
    public async Task CreateDispatch_PopulatesTenantIdFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        CreateDispatchCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDispatchCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as CreateDispatchCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new CreateDispatchCommand();

        // Act
        await _controller.CreateDispatch(command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task CreateDispatch_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new CreateDispatchCommand();

        // Act
        var result = await _controller.CreateDispatch(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateDispatchCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateDispatch_InvalidGuidTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "invalid-guid");
        var command = new CreateDispatchCommand();

        // Act
        var result = await _controller.CreateDispatch(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── GetDispatch ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDispatch_Returns501NotImplemented()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _controller.GetDispatch(id);

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }
}
