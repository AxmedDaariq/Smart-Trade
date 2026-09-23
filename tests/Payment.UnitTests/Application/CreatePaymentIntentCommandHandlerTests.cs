using Moq;
using FluentAssertions;
using FluentValidation;
using Payment.Application.Commands;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Xunit;

namespace Payment.UnitTests.Application;

public class CreatePaymentIntentCommandHandlerTests
{
    private readonly Mock<IPaymentDbContext> _dbContextMock;
    private readonly CreatePaymentIntentCommandHandler _handler;

    public CreatePaymentIntentCommandHandlerTests()
    {
        _dbContextMock = new Mock<IPaymentDbContext>();
        _dbContextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new CreatePaymentIntentCommandHandler(_dbContextMock.Object);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNonEmptyGuid()
    {
        var command = BuildCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsPaymentIntentToContext()
    {
        var command = BuildCommand();

        await _handler.Handle(command, CancellationToken.None);

        _dbContextMock.Verify(x => x.AddPaymentIntent(It.IsAny<PaymentIntent>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChanges()
    {
        var command = BuildCommand();

        await _handler.Handle(command, CancellationToken.None);

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Customs integration: CustomsCaseId is stored ──────────────────────────

    [Fact]
    public async Task Handle_CustomsCommand_StoresCustomsCaseIdInIntent()
    {
        var customsCaseId = Guid.NewGuid();
        PaymentIntent? captured = null;

        _dbContextMock
            .Setup(x => x.AddPaymentIntent(It.IsAny<PaymentIntent>()))
            .Callback<PaymentIntent>(pi => captured = pi);

        var command = BuildCommand(customsCaseId: customsCaseId, sourceSystem: "Customs");

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.CustomsCaseId.Should().Be(customsCaseId);
        captured.SourceSystem.Should().Be("Customs");
    }

    [Fact]
    public async Task Handle_CustomsCommand_StoresAssessmentIdAsSourceReferenceId()
    {
        PaymentIntent? captured = null;
        _dbContextMock
            .Setup(x => x.AddPaymentIntent(It.IsAny<PaymentIntent>()))
            .Callback<PaymentIntent>(pi => captured = pi);

        var command = BuildCommand(sourceAggregateId: "ASSESS-2026-001", sourceSystem: "Customs");

        await _handler.Handle(command, CancellationToken.None);

        captured!.SourceReferenceId.Should().Be("ASSESS-2026-001");
    }

    // ── Money mapping ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_MapsAmountAndCurrencyToIntent()
    {
        PaymentIntent? captured = null;
        _dbContextMock
            .Setup(x => x.AddPaymentIntent(It.IsAny<PaymentIntent>()))
            .Callback<PaymentIntent>(pi => captured = pi);

        var command = BuildCommand(amount: 12500m, currency: "USD");

        await _handler.Handle(command, CancellationToken.None);

        captured!.AmountRequested.Should().Be(12500m);
        captured.CurrencyCode.Should().Be("USD");
    }

    // ── DueDate passthrough ───────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WithDueDate_PropagatesDueDateToIntent()
    {
        PaymentIntent? captured = null;
        _dbContextMock
            .Setup(x => x.AddPaymentIntent(It.IsAny<PaymentIntent>()))
            .Callback<PaymentIntent>(pi => captured = pi);

        var dueDate = DateTime.UtcNow.AddDays(14);
        var command = BuildCommand(dueDate: dueDate);

        await _handler.Handle(command, CancellationToken.None);

        captured!.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
    }

    // ── Validator ─────────────────────────────────────────────────────────────

    [Fact]
    public void Validator_ValidCommand_PassesWithNoErrors()
    {
        var validator = new CreatePaymentIntentCommandValidator();
        var result = validator.Validate(BuildCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ZeroAmount_FailsValidation()
    {
        var validator = new CreatePaymentIntentCommandValidator();
        var command = BuildCommand(amount: 0m);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validator_EmptyTenantId_FailsValidation()
    {
        var validator = new CreatePaymentIntentCommandValidator();
        var command = BuildCommand();
        command.TenantId = Guid.Empty;

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void Validator_EmptySourceSystem_FailsValidation()
    {
        var validator = new CreatePaymentIntentCommandValidator();
        var command = BuildCommand();
        command.SourceSystem = "";

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SourceSystem");
    }

    [Theory]
    [InlineData("US")]      // too short
    [InlineData("USDD")]    // too long
    [InlineData("")]        // empty
    public void Validator_InvalidCurrencyCode_FailsValidation(string badCurrency)
    {
        var validator = new CreatePaymentIntentCommandValidator();
        var command = BuildCommand(currency: badCurrency);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CreatePaymentIntentCommand BuildCommand(
        decimal amount           = 5000m,
        string currency          = "USD",
        Guid? customsCaseId      = null,
        string sourceSystem      = "Customs",
        string sourceAggregateId = "ASSESS-001",
        DateTime? dueDate        = null) => new()
    {
        TenantId          = Guid.NewGuid(),
        ExternalReference = "PI-EXT-001",
        SourceSystem      = sourceSystem,
        SourceAggregateId = sourceAggregateId,
        CustomsCaseId     = customsCaseId ?? Guid.NewGuid(),
        DeclarationNumber = "DECL-2026-001",
        ECTNNumber        = "ECTN-2026-001",
        Amount            = amount,
        Currency          = currency,
        DueDate           = dueDate
    };
}
