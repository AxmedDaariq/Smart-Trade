using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AnprGate.Api.Controllers;
using AnprGate.Application.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AnprGate.UnitTests.Controllers;

public class AnprIngestionControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly AnprIngestionController _controller;

    public AnprIngestionControllerTests()
    {
        _controller = new AnprIngestionController(_mediator.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private void SetUser(string? tenantId = null, string? cameraId = null)
    {
        var claims = new List<Claim>();
        if (tenantId is not null) claims.Add(new Claim("tenant_id", tenantId));
        if (cameraId is not null) claims.Add(new Claim("camera_id", cameraId));

        _controller.ControllerContext.HttpContext.User =
            new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static IngestRecognitionCommand BuildCommand() => new()
    {
        // Deliberately wrong values: the controller must overwrite them from the token
        TenantId = Guid.Empty,
        CameraId = Guid.Empty,
        PlateText = "AB-123-CD",
        Confidence = 0.97,
        ObservedAt = new DateTime(2026, 2, 1, 8, 0, 0, DateTimeKind.Utc),
        ProviderEventId = "prov-evt-1"
    };

    [Fact]
    public async Task Ingest_WithValidClaims_ShouldReturnAccepted()
    {
        // Arrange
        var recognitionId = Guid.NewGuid();
        SetUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        _mediator
            .Setup(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recognitionId);

        // Act
        var result = await _controller.Ingest(BuildCommand());

        // Assert
        var accepted = result.Should().BeOfType<AcceptedResult>().Subject;
        accepted.StatusCode.Should().Be(StatusCodes.Status202Accepted);

        var payload = accepted.Value!;
        var status = payload.GetType().GetProperty("Status")?.GetValue(payload);
        var returnedId = payload.GetType().GetProperty("RecognitionEventId")?.GetValue(payload);
        status.Should().Be("Processing");
        returnedId.Should().Be(recognitionId);
    }

    [Fact]
    public async Task Ingest_ShouldOverrideTenantAndCameraFromTheToken()
    {
        // Arrange: cameras must never be able to claim another tenant's identity
        var tenantId = Guid.NewGuid();
        var cameraId = Guid.NewGuid();
        SetUser(tenantId.ToString(), cameraId.ToString());

        IngestRecognitionCommand? sent = null;
        _mediator
            .Setup(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((c, _) => sent = (IngestRecognitionCommand)c)
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.Ingest(BuildCommand());

        // Assert
        sent.Should().NotBeNull();
        sent!.TenantId.Should().Be(tenantId);
        sent.CameraId.Should().Be(cameraId);
    }

    [Fact]
    public async Task Ingest_ShouldForwardTheCommandPayloadToMediator()
    {
        // Arrange
        SetUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var command = BuildCommand();

        IngestRecognitionCommand? sent = null;
        _mediator
            .Setup(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((c, _) => sent = (IngestRecognitionCommand)c)
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.Ingest(command);

        // Assert: business fields must pass through untouched
        sent.Should().NotBeNull();
        sent!.PlateText.Should().Be("AB-123-CD");
        sent.Confidence.Should().Be(0.97);
        sent.ObservedAt.Should().Be(command.ObservedAt);
        sent.ProviderEventId.Should().Be("prov-evt-1");
    }

    [Fact]
    public async Task Ingest_WithoutClaims_ShouldForbid_AndNotCallMediator()
    {
        // Arrange
        SetUser();

        // Act
        var result = await _controller.Ingest(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Ingest_WithOnlyTenantClaim_ShouldForbid()
    {
        // Arrange: both tenant_id and camera_id are required for ingestion
        SetUser(tenantId: Guid.NewGuid().ToString(), cameraId: null);

        // Act
        var result = await _controller.Ingest(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Ingest_WithNonGuidClaims_ShouldForbid()
    {
        // Arrange
        SetUser(tenantId: "not-a-guid", cameraId: Guid.NewGuid().ToString());

        // Act
        var result = await _controller.Ingest(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestRecognitionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
