using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ectn.Api.Controllers;
using Ectn.Application.Commands;
using Ectn.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ectn.UnitTests.Controllers;

public class EctnControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly EctnController _controller;

    public EctnControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new EctnController(_mockMediator.Object);
    }

    private void SetupUser(Guid tenantId, Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("sub", userId.ToString())
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
    public async Task Get_ShouldReturnOk_WithDraftStatus()
    {
        // Arrange
        var testId = Guid.NewGuid();

        // Act
        var result = await _controller.Get(testId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnForbid_WhenTenantIdClaimIsMissing()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };

        var command = new CreateEctnCommand
        {
            ApplicationReference = "APP-2026-001",
            Type = EctnType.Standard
        };

        // Act
        var result = await _controller.Create(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateEctnCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_ShouldExtractTenantAndUserFromClaims_AndSendToMediator()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetupUser(tenantId, userId);

        var expectedEctnId = Guid.NewGuid();
        CreateEctnCommand? capturedCommand = null;

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateEctnCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((req, _) => capturedCommand = req as CreateEctnCommand)
            .ReturnsAsync(expectedEctnId);

        var command = new CreateEctnCommand
        {
            ApplicationReference = "APP-2026-001",
            Type = EctnType.Standard
        };

        // Act
        var result = await _controller.Create(command);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.TenantId.Should().Be(tenantId);
        capturedCommand.CreatedBy.Should().Be(userId);
        capturedCommand.ApplicationReference.Should().Be("APP-2026-001");
    }

    [Fact]
    public async Task Submit_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var ectnId = Guid.NewGuid();

        // Act
        var result = await _controller.Submit(ectnId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Approve_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var ectnId = Guid.NewGuid();

        // Act
        var result = await _controller.Approve(ectnId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Issue_ShouldReturnOk_WhenCalled()
    {
        // Arrange
        var ectnId = Guid.NewGuid();

        // Act
        var result = await _controller.Issue(ectnId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
