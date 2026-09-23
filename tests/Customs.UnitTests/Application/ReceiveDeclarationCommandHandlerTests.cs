using Moq;
using FluentAssertions;
using Customs.Application.Commands;
using Customs.Domain.Entities;
using Xunit;

namespace Customs.UnitTests.Application;

public class ReceiveDeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDbContext> _dbContextMock;
    private readonly Mock<ICustomsCaseNumberGenerator> _numberGenMock;
    private readonly ReceiveDeclarationCommandHandler _handler;

    public ReceiveDeclarationCommandHandlerTests()
    {
        _dbContextMock = new Mock<ICustomsDbContext>();
        _numberGenMock = new Mock<ICustomsCaseNumberGenerator>();

        _numberGenMock
            .Setup(x => x.Generate(It.IsAny<Guid>()))
            .Returns("CASE-2026-001");

        _dbContextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new ReceiveDeclarationCommandHandler(_dbContextMock.Object, _numberGenMock.Object);
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
    public async Task Handle_ValidCommand_AddsCustomsCaseToContext()
    {
        var command = BuildCommand();

        await _handler.Handle(command, CancellationToken.None);

        _dbContextMock.Verify(x => x.AddCustomsCase(It.IsAny<CustomsCase>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChanges()
    {
        var command = BuildCommand();

        await _handler.Handle(command, CancellationToken.None);

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Generated case number is stored ──────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_UsesCaseNumberFromGenerator()
    {
        CustomsCase? captured = null;
        _dbContextMock
            .Setup(x => x.AddCustomsCase(It.IsAny<CustomsCase>()))
            .Callback<CustomsCase>(c => captured = c);

        _numberGenMock
            .Setup(x => x.Generate(It.IsAny<Guid>()))
            .Returns("CASE-CUSTOM-999");

        var command = BuildCommand();
        await _handler.Handle(command, CancellationToken.None);

        captured!.CaseNumber.Should().Be("CASE-CUSTOM-999");
    }

    // ── Data mapping ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_MapsAllFieldsToCustomsCase()
    {
        CustomsCase? captured = null;
        _dbContextMock
            .Setup(x => x.AddCustomsCase(It.IsAny<CustomsCase>()))
            .Callback<CustomsCase>(c => captured = c);

        var command = BuildCommand();
        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.TenantId.Should().Be(command.TenantId);
        captured.DeclarationExternalId.Should().Be(command.DeclarationExternalId);
        captured.DeclarationNumber.Should().Be(command.DeclarationNumber);
        captured.ECTNExternalId.Should().Be(command.ECTNExternalId);
        captured.EctnNumber.Should().Be(command.ECTNNumber);
        captured.ProcedureCode.Should().Be(command.ProcedureCode);
        captured.ImporterReference.Should().Be(command.ImporterReference);
        captured.ExporterReference.Should().Be(command.ExporterReference);
        captured.DeclarantReference.Should().Be(command.DeclarantReference);
        captured.PortOfEntry.Should().Be(command.PortOfEntry);
        captured.PortOfExit.Should().Be(command.PortOfExit);
        captured.CountryOfOrigin.Should().Be(command.CountryOfOrigin);
        captured.CountryOfDestination.Should().Be(command.CountryOfDestination);
        captured.CreatedBy.Should().Be(command.UserId);
    }

    // ── Initial status ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_CustomsCaseStatusIsReceived()
    {
        CustomsCase? captured = null;
        _dbContextMock
            .Setup(x => x.AddCustomsCase(It.IsAny<CustomsCase>()))
            .Callback<CustomsCase>(c => captured = c);

        await _handler.Handle(BuildCommand(), CancellationToken.None);

        captured!.Status.Should().Be("Received");
    }

    // ── Validator ─────────────────────────────────────────────────────────────

    [Fact]
    public void Validator_ValidCommand_PassesWithNoErrors()
    {
        var validator = new ReceiveDeclarationCommandValidator();
        var result = validator.Validate(BuildCommand());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_EmptyTenantId_FailsValidation()
    {
        var validator = new ReceiveDeclarationCommandValidator();
        var command = BuildCommand();
        command.TenantId = Guid.Empty;

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void Validator_EmptyDeclarationNumber_FailsValidation()
    {
        var validator = new ReceiveDeclarationCommandValidator();
        var command = BuildCommand();
        command.DeclarationNumber = "";

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DeclarationNumber");
    }

    [Fact]
    public void Validator_EmptyProcedureCode_FailsValidation()
    {
        var validator = new ReceiveDeclarationCommandValidator();
        var command = BuildCommand();
        command.ProcedureCode = "";

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProcedureCode");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ReceiveDeclarationCommand BuildCommand() => new()
    {
        TenantId              = Guid.NewGuid(),
        DeclarationExternalId = "DECL-EXT-001",
        DeclarationNumber     = "DECL-2026-001",
        ECTNExternalId        = "ECTN-EXT-001",
        ECTNNumber            = "ECTN-2026-001",
        ProcedureCode         = "IM4",
        ImporterReference     = "IMP-REF",
        ExporterReference     = "EXP-REF",
        DeclarantReference    = "DEC-REF",
        PortOfEntry           = "MOG",
        PortOfExit            = "JIB",
        CountryOfOrigin       = "SO",
        CountryOfDestination  = "DJ",
        UserId                = Guid.NewGuid()
    };
}
