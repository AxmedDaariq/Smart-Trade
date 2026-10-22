using System;
using Customs.Application.Commands;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Validators;

/// <summary>
/// Unit tests for <see cref="ReceiveDeclarationCommandValidator"/>.
/// </summary>
public sealed class ReceiveDeclarationCommandValidatorTests
{
    private readonly ReceiveDeclarationCommandValidator _validator = new();

    private static ReceiveDeclarationCommand CreateValidCommand() => new()
    {
        TenantId = Guid.NewGuid(),
        DeclarationExternalId = "DEC-EXT-001",
        DeclarationNumber = "DECL-12345",
        ProcedureCode = "IM4",
        UserId = Guid.NewGuid()
    };

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyTenantId_FailsValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.TenantId = Guid.Empty;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReceiveDeclarationCommand.TenantId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyDeclarationExternalId_FailsValidation(string? value)
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeclarationExternalId = value!;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReceiveDeclarationCommand.DeclarationExternalId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyDeclarationNumber_FailsValidation(string? value)
    {
        // Arrange
        var command = CreateValidCommand();
        command.DeclarationNumber = value!;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReceiveDeclarationCommand.DeclarationNumber));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyProcedureCode_FailsValidation(string? value)
    {
        // Arrange
        var command = CreateValidCommand();
        command.ProcedureCode = value!;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReceiveDeclarationCommand.ProcedureCode));
    }

    [Fact]
    public void Validate_EmptyUserId_FailsValidation()
    {
        // Arrange
        var command = CreateValidCommand();
        command.UserId = Guid.Empty;

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReceiveDeclarationCommand.UserId));
    }
}
