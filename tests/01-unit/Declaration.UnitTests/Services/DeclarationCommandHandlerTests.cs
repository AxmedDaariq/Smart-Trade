using System;
using System.Threading;
using System.Threading.Tasks;
using Declaration.Application.Commands;
using Declaration.Application.Interfaces;
using Declaration.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Declaration.UnitTests.Services;

/// <summary>
/// Unit tests for Declaration command handlers and validators.
/// Validates application-layer logic using mocked repository and service clients.
/// </summary>
public sealed class DeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDeclarationRepository> _repoMock = new();
    private readonly Mock<IEctnServiceClient> _ectnClientMock = new();

    // ─── CreateDeclarationCommandHandler ─────────────────────────────────────

    [Fact]
    public async Task CreateHandler_ValidCommand_AddsDeclarationAndReturnsId()
    {
        // Arrange
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<CustomsDeclaration>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateDeclarationCommandHandler(_repoMock.Object);
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-001",
            DeclarationType = Declaration.Domain.Enums.DeclarationType.Import,
            ProcedureCode = "IM4",
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.Is<CustomsDeclaration>(d =>
            d.TenantId == command.TenantId
        )), Times.Once);
    }

    // ─── SubmitDeclarationCommandHandler ─────────────────────────────────────

    [Fact]
    public async Task SubmitHandler_DeclarationNotFound_Throws()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((CustomsDeclaration?)null);

        var handler = new SubmitDeclarationCommandHandler(_repoMock.Object, _ectnClientMock.Object);
        var command = new SubmitDeclarationCommand
        {
            DeclarationId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            EctnReference = "ECTN-001"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*DECLARATION_NOT_FOUND*");
    }

    [Fact]
    public async Task SubmitHandler_InvalidEctn_ThrowsInvalidOperation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var declaration = new CustomsDeclaration(
            tenantId, "APP-001",
            Declaration.Domain.Enums.DeclarationType.Import,
            "IM4", Guid.NewGuid());

        _repoMock
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), tenantId))
            .ReturnsAsync(declaration);

        _ectnClientMock
            .Setup(e => e.ValidateEctnAsync("BAD-ECTN", tenantId))
            .ReturnsAsync(false);

        var handler = new SubmitDeclarationCommandHandler(_repoMock.Object, _ectnClientMock.Object);
        var command = new SubmitDeclarationCommand
        {
            DeclarationId = declaration.Id,
            TenantId = tenantId,
            EctnReference = "BAD-ECTN"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*ECTN_INVALID*");
    }

    // ─── AcceptDeclarationCommandHandler ─────────────────────────────────────

    [Fact]
    public async Task AcceptHandler_DeclarationNotFound_Throws()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((CustomsDeclaration?)null);

        var handler = new AcceptDeclarationCommandHandler(_repoMock.Object);
        var command = new AcceptDeclarationCommand
        {
            DeclarationId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            DeclarationNumber = "DEC-001"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("*DECLARATION_NOT_FOUND*");
    }

    // ─── RejectDeclarationCommandHandler ─────────────────────────────────────

    [Fact]
    public async Task RejectHandler_DeclarationNotFound_Throws()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((CustomsDeclaration?)null);

        var handler = new RejectDeclarationCommandHandler(_repoMock.Object);
        var command = new RejectDeclarationCommand
        {
            DeclarationId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ReasonCode = "DOC-MISSING",
            Reason = "Missing invoice"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("*DECLARATION_NOT_FOUND*");
    }

    [Fact]
    public async Task RejectHandler_MissingReasonCode_Throws()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var declaration = new CustomsDeclaration(
            tenantId, "APP-001",
            Declaration.Domain.Enums.DeclarationType.Import,
            "IM4", Guid.NewGuid());

        _repoMock
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), tenantId))
            .ReturnsAsync(declaration);

        var handler = new RejectDeclarationCommandHandler(_repoMock.Object);
        var command = new RejectDeclarationCommand
        {
            DeclarationId = declaration.Id,
            TenantId = tenantId,
            ReasonCode = "",
            Reason = "Missing docs"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("*REASON_REQUIRED*");
    }

    // ─── CreateDeclarationCommandValidator ───────────────────────────────────

    [Fact]
    public void Validator_ValidCommand_Passes()
    {
        var validator = new CreateDeclarationCommandValidator();
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-REF",
            ProcedureCode = "IM4",
            CreatedBy = Guid.NewGuid(),
            DeclarationType = Declaration.Domain.Enums.DeclarationType.Import
        };

        var result = validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_EmptyTenantId_Fails()
    {
        var validator = new CreateDeclarationCommandValidator();
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.Empty,
            ApplicationReference = "APP-REF",
            ProcedureCode = "IM4",
            CreatedBy = Guid.NewGuid()
        };

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public void Validator_EmptyApplicationReference_Fails()
    {
        var validator = new CreateDeclarationCommandValidator();
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "",
            ProcedureCode = "IM4",
            CreatedBy = Guid.NewGuid()
        };

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ApplicationReference");
    }

    [Fact]
    public void Validator_EmptyProcedureCode_Fails()
    {
        var validator = new CreateDeclarationCommandValidator();
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-REF",
            ProcedureCode = "",
            CreatedBy = Guid.NewGuid()
        };

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProcedureCode");
    }

    [Fact]
    public void Validator_EmptyCreatedBy_Fails()
    {
        var validator = new CreateDeclarationCommandValidator();
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-REF",
            ProcedureCode = "IM4",
            CreatedBy = Guid.Empty
        };

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CreatedBy");
    }
}
