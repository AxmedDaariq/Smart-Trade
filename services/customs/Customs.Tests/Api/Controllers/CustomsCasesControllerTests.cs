using System;
using System.Threading.Tasks;
using Customs.Api.Controllers;
using Customs.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Customs.Tests.Api.Controllers;

public class CustomsCasesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly CustomsCasesController _controller;

    public CustomsCasesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new CustomsCasesController(_mockMediator.Object);
    }

    private void SetUserClaims(Guid tenantId, Guid userId)
    {
        var claims = new[]
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("sub", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateCase_Should_Return_201_Created_With_CaseId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var newCaseId = Guid.NewGuid();

        SetUserClaims(tenantId, userId);

        _mockMediator
            .Setup(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), default))
            .ReturnsAsync(newCaseId);

        var command = new ReceiveDeclarationCommand
        {
            DeclarationExternalId = "DEC-EXT-001",
            DeclarationNumber = "1000001",
            ProcedureCode = "IM4",
            PortOfEntry = "GEN",
            CountryOfOrigin = "CN",
            CountryOfDestination = "SO"
        };

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.NotNull(createdResult.Value);
    }

    [Fact]
    public async Task CreateCase_Should_Set_TenantId_From_Claims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        SetUserClaims(tenantId, userId);

        ReceiveDeclarationCommand? capturedCommand = null;
        _mockMediator
            .Setup(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), default))
            .Callback<IRequest<Guid>, System.Threading.CancellationToken>((cmd, _) =>
                capturedCommand = cmd as ReceiveDeclarationCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new ReceiveDeclarationCommand
        {
            DeclarationExternalId = "DEC-EXT-002",
            DeclarationNumber = "2000002",
            ProcedureCode = "EX3"
        };

        // Act
        await _controller.CreateCase(command);

        // Assert: TenantId was populated from JWT claims
        Assert.NotNull(capturedCommand);
        Assert.Equal(tenantId, capturedCommand!.TenantId);
        Assert.Equal(userId, capturedCommand.UserId);
    }

    [Fact]
    public async Task CreateCase_Should_Return_Forbid_When_TenantId_Claim_Missing()
    {
        // Arrange - No claims set (no tenant_id claim)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var command = new ReceiveDeclarationCommand
        {
            DeclarationNumber = "3000003"
        };

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        Assert.IsType<ForbidResult>(result);

        // Mediator should NOT have been called
        _mockMediator.Verify(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task GetCase_Should_Return_501_NotImplemented()
    {
        // Arrange
        var caseId = Guid.NewGuid();

        // Act
        var result = await _controller.GetCase(caseId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(501, problemResult.StatusCode);
    }

    [Fact]
    public async Task StartRiskAssessment_Should_Return_501_NotImplemented()
    {
        // Arrange
        var caseId = Guid.NewGuid();

        // Act
        var result = await _controller.StartRiskAssessment(caseId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(501, problemResult.StatusCode);
    }
}
