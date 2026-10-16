using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Shipping.Application.Commands;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;
using Xunit;

namespace Shipping.UnitTests.Services;

/// <summary>
/// Unit tests for the Shipping command handlers (application-layer logic).
/// Validates command handling behaviour using mocked persistence.
/// </summary>
public sealed class UpdateShipmentStatusHandlerTests
{
    private readonly Mock<IShippingQueryContext> _contextMock = new();

    // ─── UpdateShipmentStatusCommandHandler ──────────────────────────────────

    [Fact]
    public async Task Handle_ShipmentNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _contextMock
            .Setup(c => c.GetShipmentWithHistoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Shipping.Domain.Entities.Shipment?)null);

        var handler = new UpdateShipmentStatusCommandHandler(_contextMock.Object);
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            NewStatus = ShipmentStatus.InTransit,
            UserId = Guid.NewGuid()
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*not found*");
    }

    [Fact]
    public void Validate_EmptyTenantId_Fails()
    {
        // Arrange
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.Empty,
            ShipmentId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void Validate_EmptyShipmentId_Fails()
    {
        // Arrange
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.Empty,
            UserId = Guid.NewGuid()
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ShipmentId");
    }

    [Fact]
    public void Validate_EmptyUserId_Fails()
    {
        // Arrange
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            UserId = Guid.Empty
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        // Arrange
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            NewStatus = ShipmentStatus.InTransit,
            Reason = "Departed port"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ReasonExceedsMaxLength_Fails()
    {
        // Arrange
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Reason = new string('x', 501)
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Reason");
    }
}
