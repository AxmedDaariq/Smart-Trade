using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;
using Ectn.Application.Commands;
using Ectn.Domain.Enums;

namespace Ectn.UnitTests.Application;

public class CreateEctnCommandValidatorTests
{
    private readonly CreateEctnCommandValidator _validator = new();

    private static CreateEctnCommand ValidCommand() => new()
    {
        TenantId = Guid.NewGuid(),
        ApplicationReference = "APP-REF-VALID-001",
        Type = EctnType.Standard,
        CreatedBy = Guid.NewGuid()
    };

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        var cmd = ValidCommand();
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_TenantId_IsEmpty()
    {
        var cmd = ValidCommand();
        cmd.TenantId = Guid.Empty;

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(c => c.TenantId)
              .WithErrorMessage("TenantId is required.");
    }

    [Fact]
    public void Should_Fail_When_ApplicationReference_IsEmpty()
    {
        var cmd = ValidCommand();
        cmd.ApplicationReference = "";

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(c => c.ApplicationReference);
    }

    [Fact]
    public void Should_Fail_When_ApplicationReference_ExceedsMaxLength()
    {
        var cmd = ValidCommand();
        cmd.ApplicationReference = new string('A', 101); // max is 100

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(c => c.ApplicationReference);
    }

    [Fact]
    public void Should_Fail_When_CreatedBy_IsEmpty()
    {
        var cmd = ValidCommand();
        cmd.CreatedBy = Guid.Empty;

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(c => c.CreatedBy);
    }

    [Fact]
    public void Should_Fail_When_Type_IsOutOfRange()
    {
        var cmd = ValidCommand();
        cmd.Type = (EctnType)999; // invalid enum value

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(c => c.Type);
    }

    [Theory]
    [InlineData(EctnType.Standard)]
    [InlineData(EctnType.Transit)]
    [InlineData(EctnType.Consolidated)]
    [InlineData(EctnType.Containerized)]
    [InlineData(EctnType.Bulk)]
    [InlineData(EctnType.FreeZone)]
    [InlineData(EctnType.Amendment)]
    [InlineData(EctnType.ReExport)]
    public void Should_Pass_For_All_Valid_EctnTypes(EctnType type)
    {
        var cmd = ValidCommand();
        cmd.Type = type;

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(c => c.Type);
    }
}
