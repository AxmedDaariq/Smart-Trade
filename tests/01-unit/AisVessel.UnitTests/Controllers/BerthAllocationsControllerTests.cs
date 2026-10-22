using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AisVessel.Api.Controllers;
using AisVessel.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AisVessel.UnitTests.Controllers;

public class BerthAllocationsControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly BerthAllocationsController _controller;

    public BerthAllocationsControllerTests()
    {
        _controller = new BerthAllocationsController(_mediator.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private void SetUser(string? tenantId = null)
    {
        var claims = new List<Claim>();
        if (tenantId is not null) claims.Add(new Claim("tenant_id", tenantId));

        _controller.ControllerContext.HttpContext.User =
            new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static AllocateBerthCommand BuildCommand() => new()
    {
        // Deliberately wrong tenant: the controller must overwrite it from the token
        TenantId = Guid.Empty,
        PortCallId = Guid.NewGuid(),
        BerthId = Guid.NewGuid()
    };

    [Fact]
    public async Task AllocateBerth_WithTenantClaim_ShouldReturnOkWithPortCallId()
    {
        // Arrange
        var portCallId = Guid.NewGuid();
        SetUser(Guid.NewGuid().ToString());
        _mediator
            .Setup(m => m.Send(It.IsAny<AllocateBerthCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(portCallId);

        // Act
        var result = await _controller.AllocateBerth(BuildCommand());

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var payload = ok.Value!;
        var status = payload.GetType().GetProperty("Status")?.GetValue(payload);
        var returnedId = payload.GetType().GetProperty("PortCallId")?.GetValue(payload);
        status.Should().Be("BerthAllocated");
        returnedId.Should().Be(portCallId);
    }

    [Fact]
    public async Task AllocateBerth_ShouldOverrideTenantFromTheToken()
    {
        // Arrange: clients must never allocate on behalf of another tenant
        var tenantId = Guid.NewGuid();
        SetUser(tenantId.ToString());

        AllocateBerthCommand? sent = null;
        _mediator
            .Setup(m => m.Send(It.IsAny<AllocateBerthCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((c, _) => sent = (AllocateBerthCommand)c)
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.AllocateBerth(BuildCommand());

        // Assert
        sent.Should().NotBeNull();
        sent!.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task AllocateBerth_WithoutTenantClaim_ShouldForbid_AndNotCallMediator()
    {
        // Arrange
        SetUser();

        // Act
        var result = await _controller.AllocateBerth(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<AllocateBerthCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AllocateBerth_WithNonGuidTenantClaim_ShouldForbid()
    {
        // Arrange
        SetUser(tenantId: "not-a-guid");

        // Act
        var result = await _controller.AllocateBerth(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<AllocateBerthCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
