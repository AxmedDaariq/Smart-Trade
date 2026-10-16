using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GpsTelematics.Api.Controllers;
using GpsTelematics.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GpsTelematics.UnitTests.Controllers;

public class TelemetryIngestionControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly TelemetryIngestionController _controller;

    public TelemetryIngestionControllerTests()
    {
        _controller = new TelemetryIngestionController(_mediator.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    private void SetUser(string? tenantId = null, string? deviceId = null)
    {
        var claims = new List<Claim>();
        if (tenantId is not null) claims.Add(new Claim("tenant_id", tenantId));
        if (deviceId is not null) claims.Add(new Claim("device_id", deviceId));

        _controller.ControllerContext.HttpContext.User =
            new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }

    private static IngestTelemetryCommand BuildCommand() => new()
    {
        // Deliberately wrong values: the controller must overwrite them from the token
        TenantId = Guid.Empty,
        DeviceId = Guid.Empty,
        Latitude = 11.5919,
        Longitude = 43.1456,
        Speed = 30,
        Heading = 90,
        RecordedAt = new DateTime(2026, 2, 1, 8, 0, 0, DateTimeKind.Utc),
        SequenceNumber = 42
    };

    [Fact]
    public async Task IngestTelemetry_WithValidClaims_ShouldReturnAccepted()
    {
        // Arrange
        var positionId = Guid.NewGuid();
        SetUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        _mediator
            .Setup(m => m.Send(It.IsAny<IngestTelemetryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positionId);

        // Act
        var result = await _controller.IngestTelemetry(BuildCommand());

        // Assert
        result.Should().BeOfType<AcceptedResult>();
        var accepted = result as AcceptedResult;
        accepted!.StatusCode.Should().Be(StatusCodes.Status202Accepted);

        var payload = accepted.Value!;
        var status = payload.GetType().GetProperty("Status")?.GetValue(payload);
        var returnedId = payload.GetType().GetProperty("PositionId")?.GetValue(payload);
        status.Should().Be("Processed");
        returnedId.Should().Be(positionId);
    }

    [Fact]
    public async Task IngestTelemetry_ShouldOverrideTenantAndDeviceFromTheToken()
    {
        // Arrange: clients must never be able to claim another tenant's identity
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        SetUser(tenantId.ToString(), deviceId.ToString());

        IngestTelemetryCommand? sent = null;
        _mediator
            .Setup(m => m.Send(It.IsAny<IngestTelemetryCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((c, _) => sent = (IngestTelemetryCommand)c)
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.IngestTelemetry(BuildCommand());

        // Assert
        sent.Should().NotBeNull();
        sent!.TenantId.Should().Be(tenantId);
        sent.DeviceId.Should().Be(deviceId);
    }

    [Fact]
    public async Task IngestTelemetry_WithoutClaims_ShouldForbid_AndNotCallMediator()
    {
        // Arrange
        SetUser();

        // Act
        var result = await _controller.IngestTelemetry(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestTelemetryCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IngestTelemetry_WithOnlyTenantClaim_ShouldForbid()
    {
        // Arrange: both tenant_id and device_id are required for ingestion
        SetUser(tenantId: Guid.NewGuid().ToString(), deviceId: null);

        // Act
        var result = await _controller.IngestTelemetry(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestTelemetryCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IngestTelemetry_WithNonGuidClaims_ShouldForbid()
    {
        // Arrange
        SetUser(tenantId: "not-a-guid", deviceId: Guid.NewGuid().ToString());

        // Act
        var result = await _controller.IngestTelemetry(BuildCommand());

        // Assert
        result.Should().BeOfType<ForbidResult>();
        _mediator.Verify(m => m.Send(It.IsAny<IngestTelemetryCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
