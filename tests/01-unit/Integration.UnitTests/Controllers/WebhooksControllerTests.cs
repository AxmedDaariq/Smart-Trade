using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Integration.Api.Controllers;
using Integration.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Integration.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="WebhooksController"/> using mocked <see cref="IMediator"/>.
/// Validates HTTP 202/401 responses, header extraction, and raw-body reading.
/// The webhook endpoint is [AllowAnonymous] — authentication is via HMAC signature.
/// </summary>
public sealed class WebhooksControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly WebhooksController _controller;

    public WebhooksControllerTests()
    {
        _controller = new WebhooksController(_mediatorMock.Object);
    }

    /// <summary>
    /// Creates a fake HTTP context with a raw body and optional signature/timestamp headers.
    /// </summary>
    private void SetHttpContext(
        string body = "{}",
        string? signatureHeader = null,
        string? timestampHeader = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        httpContext.Request.ContentType = "application/json";

        if (signatureHeader is not null)
            httpContext.Request.Headers["X-Hub-Signature-256"] = signatureHeader;

        if (timestampHeader is not null)
            httpContext.Request.Headers["X-Timestamp"] = timestampHeader;

        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    // ─── ReceiveWebhook ──────────────────────────────────────────────────────

    [Fact]
    public async Task ReceiveWebhook_ValidSignature_Returns202Accepted()
    {
        // Arrange
        var partnerCode = "maersk";
        var payload = "{\"event\":\"shipment.created\"}";
        var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        SetHttpContext(body: payload, signatureHeader: "sha256=abc123", timestampHeader: ts);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ReceiveWebhook(partnerCode);

        // Assert
        var accepted = result.Should().BeOfType<AcceptedResult>().Subject;
        accepted.StatusCode.Should().Be(202);
    }

    [Fact]
    public async Task ReceiveWebhook_InvalidSignature_Returns401Unauthorized()
    {
        // Arrange
        SetHttpContext(body: "{}", signatureHeader: "sha256=bad", timestampHeader: "1234567890");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ReceiveWebhook("unknown-partner");

        // Assert
        var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ReceiveWebhook_ExtractsPartnerCodeFromRoute()
    {
        // Arrange
        var partnerCode = "dhl-express";
        SetHttpContext(body: "{\"event\":\"pickup.scheduled\"}");

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook(partnerCode);

        // Assert
        captured.Should().NotBeNull();
        captured!.PartnerCode.Should().Be("dhl-express");
    }

    [Fact]
    public async Task ReceiveWebhook_ExtractsRawPayloadFromBody()
    {
        // Arrange
        var payload = "{\"tracking_number\":\"DHL-12345\"}";
        SetHttpContext(body: payload);

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        captured.Should().NotBeNull();
        captured!.Payload.Should().Be(payload);
    }

    [Fact]
    public async Task ReceiveWebhook_ExtractsSignatureFromHeader()
    {
        // Arrange
        var signature = "sha256=abcdef1234567890";
        SetHttpContext(signatureHeader: signature);

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        captured.Should().NotBeNull();
        captured!.Signature.Should().Be(signature);
    }

    [Fact]
    public async Task ReceiveWebhook_ExtractsTimestampFromHeader()
    {
        // Arrange
        var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        SetHttpContext(timestampHeader: ts.ToString());

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        captured.Should().NotBeNull();
        captured!.Timestamp.Should().NotBeNull();
        captured.Timestamp!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ReceiveWebhook_MissingTimestampHeader_SetsTimestampToNull()
    {
        // Arrange
        SetHttpContext(timestampHeader: null);

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        captured.Should().NotBeNull();
        captured!.Timestamp.Should().BeNull();
    }

    [Fact]
    public async Task ReceiveWebhook_SetsTenantIdToEmpty()
    {
        // Arrange – webhooks resolve tenant from partner mapping, not from JWT
        SetHttpContext();

        ProcessWebhookCommand? captured = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<bool>, CancellationToken>((cmd, _) => captured = cmd as ProcessWebhookCommand)
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task ReceiveWebhook_InvokesMediatrExactlyOnce()
    {
        // Arrange
        SetHttpContext();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.ReceiveWebhook("partner");

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<ProcessWebhookCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
