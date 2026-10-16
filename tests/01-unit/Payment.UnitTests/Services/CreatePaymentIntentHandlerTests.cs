using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Payment.Application.Commands;
using Xunit;

namespace Payment.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CreatePaymentIntentCommandHandler"/> and
/// <see cref="CreatePaymentIntentCommandValidator"/>.
/// Validates command handling and FluentValidation rules.
/// </summary>
public sealed class CreatePaymentIntentHandlerTests
{
    private readonly Mock<IPaymentDbContext> _contextMock = new();

    // ─── Handler ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_CreatesIntentAndReturnsId()
    {
        // Arrange
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreatePaymentIntentCommandHandler(_contextMock.Object);
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 2500.00m,
            Currency = "USD",
            SourceSystem = "Customs",
            SourceAggregateId = "AGG-001",
            CustomsCaseId = Guid.NewGuid(),
            DeclarationNumber = "DEC-001",
            ECTNNumber = "ECTN-2026-001"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty("the handler should return a new intent ID");
        _contextMock.Verify(c => c.AddPaymentIntent(It.IsAny<Payment.Domain.Entities.PaymentIntent>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleCalls_ReturnUniqueIds()
    {
        // Arrange
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreatePaymentIntentCommandHandler(_contextMock.Object);
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
            SourceSystem = "Test"
        };

        // Act
        var id1 = await handler.Handle(command, CancellationToken.None);
        var id2 = await handler.Handle(command, CancellationToken.None);

        // Assert
        id1.Should().NotBe(id2);
    }

    // ─── Validator ───────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ValidCommand_Passes()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 500m,
            Currency = "USD",
            SourceSystem = "Customs"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyTenantId_Fails()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.Empty,
            Amount = 500m,
            Currency = "USD",
            SourceSystem = "Test"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void Validate_ZeroAmount_Fails()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 0m,
            Currency = "USD",
            SourceSystem = "Test"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_NegativeAmount_Fails()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = -10m,
            Currency = "USD",
            SourceSystem = "Test"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_InvalidCurrency_Fails()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "DOLLAR", // not a 3-letter code
            SourceSystem = "Test"
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }

    [Fact]
    public void Validate_EmptySourceSystem_Fails()
    {
        // Arrange
        var validator = new CreatePaymentIntentCommandValidator();
        var command = new CreatePaymentIntentCommand
        {
            TenantId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "USD",
            SourceSystem = ""
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SourceSystem");
    }
}
