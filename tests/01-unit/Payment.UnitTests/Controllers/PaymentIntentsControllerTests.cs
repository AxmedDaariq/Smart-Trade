using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Payment.Api.Controllers;
using Payment.Application.Commands;
using Xunit;

namespace Payment.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="PaymentIntentsController"/> using mocked <see cref="IMediator"/>.
/// Tests HTTP status codes, claims extraction, and controller behavior in isolation.
/// </summary>
public sealed class PaymentIntentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly PaymentIntentsController _controller;

    public PaymentIntentsControllerTests()
    {
        _controller = new PaymentIntentsController(_mediatorMock.Object);
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

    // ─── CreateIntent ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateIntent_WithValidClaims_Returns201WithIntentId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        var expectedId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreatePaymentIntentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var command = new CreatePaymentIntentCommand
        {
            Amount = 1500.00m,
            Currency = "USD",
            SourceSystem = "Customs"
        };

        // Act
        var result = await _controller.CreateIntent(command);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
        created.ActionName.Should().Be(nameof(PaymentIntentsController.GetIntent));
    }

    [Fact]
    public async Task CreateIntent_PopulatesTenantIdFromClaims()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        SetControllerContext(tenantId.ToString());

        CreatePaymentIntentCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreatePaymentIntentCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => captured = cmd as CreatePaymentIntentCommand)
            .ReturnsAsync(Guid.NewGuid());

        var command = new CreatePaymentIntentCommand
        {
            Amount = 100m,
            Currency = "USD",
            SourceSystem = "Declaration"
        };

        // Act
        await _controller.CreateIntent(command);

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task CreateIntent_MissingTenantIdClaim_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: null);
        var command = new CreatePaymentIntentCommand();

        // Act
        var result = await _controller.CreateIntent(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreatePaymentIntentCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateIntent_InvalidGuidTenantId_ReturnsForbid()
    {
        // Arrange
        SetControllerContext(tenantIdClaim: "not-a-guid");
        var command = new CreatePaymentIntentCommand();

        // Act
        var result = await _controller.CreateIntent(command);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─── GetIntent ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetIntent_Returns501NotImplemented()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _controller.GetIntent(id);

        // Assert
        var problem = result.Should().BeOfType<ObjectResult>().Subject;
        problem.StatusCode.Should().Be(501);
    }
}
