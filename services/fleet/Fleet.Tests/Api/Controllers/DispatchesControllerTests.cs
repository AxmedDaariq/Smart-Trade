using System;
using System.Threading.Tasks;
using Fleet.Api.Controllers;
using Fleet.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Fleet.Tests.Api.Controllers;

public class DispatchesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly DispatchesController _controller;

    public DispatchesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new DispatchesController(_mockMediator.Object);
    }

    private void SetUserClaims(Guid tenantId)
    {
        var claims = new[] { new Claim("tenant_id", tenantId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task CreateDispatch_Should_Return_201_Created()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var dispatchId = Guid.NewGuid();
        SetUserClaims(tenantId);

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateDispatchCommand>(), default))
            .ReturnsAsync(dispatchId);

        var command = new CreateDispatchCommand
        {
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            CargoReference = "CARGO-2026-001"
        };

        // Act
        var result = await _controller.CreateDispatch(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task CreateDispatch_Should_Set_TenantId_From_Claims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetUserClaims(tenantId);

        CreateDispatchCommand? capturedCommand = null;
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateDispatchCommand>(), default))
            .Callback<IRequest<Guid>, System.Threading.CancellationToken>((cmd, _) =>
                capturedCommand = cmd as CreateDispatchCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new CreateDispatchCommand
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        // Act
        await _controller.CreateDispatch(command);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(tenantId, capturedCommand!.TenantId);
    }

    [Fact]
    public async Task CreateDispatch_Should_Return_Forbid_When_No_TenantId_Claim()
    {
        // Arrange - No claims
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await _controller.CreateDispatch(new CreateDispatchCommand());

        // Assert
        Assert.IsType<ForbidResult>(result);
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateDispatchCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task GetDispatch_Should_Return_501_NotImplemented()
    {
        // Act
        var result = await _controller.GetDispatch(Guid.NewGuid());

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(501, problemResult.StatusCode);
    }
}
