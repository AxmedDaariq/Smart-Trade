using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Declaration.Api.Controllers;
using Declaration.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Declaration.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="DeclarationsController"/> using mocked <see cref="IMediator"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class DeclarationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly DeclarationsController _controller;

    public DeclarationsControllerTests()
    {
        _controller = new DeclarationsController(_mediatorMock.Object);
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

    // ─── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_WithValidClaims_Returns201WithDeclarationId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        var expectedId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var command = new CreateDeclarationCommand
        {
            ApplicationReference = "APP-REF-001",
            ProcedureCode = "IM4"
        };

        // Act
        var result = await _controller.Create(command);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(DeclarationsController.Get));

        _mediatorMock.Verify(m => m.Send(It.Is<CreateDeclarationCommand>(c =>
            c.TenantId == tenantId &&
            c.CreatedBy == userId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_PopulatesTenantIdAndCreatedByFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        CreateDeclarationCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as CreateDeclarationCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new CreateDeclarationCommand { ApplicationReference = "REF-001", ProcedureCode = "IM4" };

        // Act
        await _controller.Create(command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
        captured.CreatedBy.Should().Be(userId);
    }

    [Fact]
    public async Task Create_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange – sub present, tenant_id missing
        SetControllerContext(tenantIdClaim: null, subClaim: Guid.NewGuid().ToString());
        var command = new CreateDeclarationCommand();

        // Act
        var result = await _controller.Create(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateDeclarationCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_MissingSubClaim_ReturnsForbid()
    {
        // Arrange – tenant_id present, sub missing → TryGetActor returns false
        SetControllerContext(tenantIdClaim: Guid.NewGuid().ToString(), subClaim: null);
        var command = new CreateDeclarationCommand();

        // Act
        var result = await _controller.Create(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Create_InvalidGuidClaims_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "bad", subClaim: "also-bad");
        var command = new CreateDeclarationCommand();

        // Act
        var result = await _controller.Create(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── Get ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_Returns501NotImplemented()
    {
        // Act
        var result = await _controller.Get(Guid.NewGuid());

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }

    // ─── Submit ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Submit_WithValidClaims_Returns200()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SubmitDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new SubmitDeclarationCommand { EctnReference = "ECTN-001" };
        var declarationId = Guid.NewGuid();

        // Act
        var result = await _controller.Submit(declarationId, command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(m => m.Send(It.Is<SubmitDeclarationCommand>(c =>
            c.TenantId == tenantId &&
            c.DeclarationId == declarationId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Submit_MissingClaims_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new SubmitDeclarationCommand();

        // Act
        var result = await _controller.Submit(Guid.NewGuid(), command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── Accept ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Accept_WithValidClaims_Returns200()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AcceptDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var declarationId = Guid.NewGuid();
        var command = new AcceptDeclarationCommand { DeclarationNumber = "DEC-001" };

        // Act
        var result = await _controller.Accept(declarationId, command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(m => m.Send(It.Is<AcceptDeclarationCommand>(c =>
            c.TenantId == tenantId &&
            c.DeclarationId == declarationId &&
            c.UserId == userId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── Reject ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Reject_WithValidClaims_Returns200()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString(), userId.ToString());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RejectDeclarationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var declarationId = Guid.NewGuid();
        var command = new RejectDeclarationCommand { ReasonCode = "DOC-MISSING", Reason = "Missing invoice" };

        // Act
        var result = await _controller.Reject(declarationId, command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(m => m.Send(It.Is<RejectDeclarationCommand>(c =>
            c.TenantId == tenantId &&
            c.DeclarationId == declarationId &&
            c.UserId == userId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── StartReview ─────────────────────────────────────────────────────────

    [Fact]
    public async Task StartReview_Returns501NotImplemented()
    {
        // Act
        var result = await _controller.StartReview(Guid.NewGuid());

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }
}
