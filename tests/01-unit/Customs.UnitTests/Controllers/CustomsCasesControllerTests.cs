using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Customs.Api.Controllers;
using Customs.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Customs.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="CustomsCasesController"/> using mocked <see cref="IMediator"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class CustomsCasesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly CustomsCasesController _controller;

    public CustomsCasesControllerTests()
    {
        _controller = new CustomsCasesController(_mediatorMock.Object);
    }

    private void SetControllerContext(string? tenantIdClaim = null, string? subClaim = null)
    {
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>();

        if (tenantIdClaim is not null)
        {
            claims.Add(new Claim("tenant_id", tenantIdClaim));
        }

        if (subClaim is not null)
        {
            claims.Add(new Claim("sub", subClaim));
        }

        if (claims.Count > 0)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static ReceiveDeclarationCommand CreateSampleCommand() => new()
    {
        DeclarationExternalId = "DEC-EXT-001",
        DeclarationNumber = "1000001",
        ECTNExternalId = "ECTN-001",
        ECTNNumber = "ECTN-2026-001",
        ProcedureCode = "IM4",
        ImporterReference = "IMP-100",
        ExporterReference = "EXP-200",
        DeclarantReference = "DCL-300",
        PortOfEntry = "BERBERA",
        PortOfExit = "DUBAI",
        CountryOfOrigin = "UAE",
        CountryOfDestination = "SO"
    };

    [Fact]
    public async Task CreateCase_WithValidClaims_Returns201Created_WithCaseId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedCaseId = Guid.NewGuid();

        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCaseId);

        var command = CreateSampleCommand();

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(CustomsCasesController.GetCase));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(expectedCaseId);

        var responseData = createdResult.Value;
        responseData.Should().NotBeNull();

        _mediatorMock.Verify(m => m.Send(It.Is<ReceiveDeclarationCommand>(c =>
            c.TenantId == tenantId &&
            c.UserId == userId &&
            c.DeclarationNumber == "1000001"
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCase_PopulatesTenantIdAndUserId_FromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        ReceiveDeclarationCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as ReceiveDeclarationCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = CreateSampleCommand();

        // Act
        await _controller.CreateCase(command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
        captured.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateCase_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange: sub claim present but no tenant_id
        SetControllerContext(tenantIdClaim: null, subClaim: Guid.NewGuid().ToString());
        var command = CreateSampleCommand();

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCase_InvalidGuidTenantIdClaim_ReturnsForbid()
    {
        // Arrange: non-guid tenant_id
        SetControllerContext(tenantIdClaim: "not-a-valid-guid", subClaim: Guid.NewGuid().ToString());
        var command = CreateSampleCommand();

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateCase_WithoutSubClaim_ProceedsWithEmptyOrExistingUserId()
    {
        // Arrange: tenant_id present, sub claim missing
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), subClaim: null);

        var expectedCaseId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ReceiveDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCaseId);

        var initialUserId = Guid.NewGuid();
        var command = CreateSampleCommand();
        command.UserId = initialUserId;

        // Act
        var result = await _controller.CreateCase(command);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        command.UserId.Should().Be(initialUserId, "UserId is preserved if sub claim is not present");
    }

    [Fact]
    public async Task GetCase_Returns501NotImplemented()
    {
        // Arrange
        var caseId = Guid.NewGuid();

        // Act
        var result = await _controller.GetCase(caseId);

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }

    [Fact]
    public async Task StartRiskAssessment_Returns501NotImplemented()
    {
        // Arrange
        var caseId = Guid.NewGuid();

        // Act
        var result = await _controller.StartRiskAssessment(caseId);

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }
}
