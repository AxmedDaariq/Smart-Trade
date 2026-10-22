using System;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;

namespace Declaration.Tests.Entities;

public class CustomsDeclarationTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsDraftStatusAndFields()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var declaration = new CustomsDeclaration(tenantId, "APP-REF-1", DeclarationType.Import, "PROC-1", userId);

        Assert.Equal(tenantId, declaration.TenantId);
        Assert.Equal("APP-REF-1", declaration.ApplicationReference);
        Assert.Equal(DeclarationType.Import.ToString(), declaration.DeclarationType);
        Assert.Equal("PROC-1", declaration.ProcedureCode);
        Assert.Equal(userId, declaration.CreatedBy);
        Assert.Equal(DeclarationStatus.Draft.ToString(), declaration.Status);
        Assert.NotEqual(Guid.Empty, declaration.Id);
    }

    [Fact]
    public void Submit_SetsSubmittedStatusAndTimestamp()
    {
        var declaration = NewDraft();
        var userId = Guid.NewGuid();

        declaration.Submit(userId);

        Assert.Equal(DeclarationStatus.Submitted.ToString(), declaration.Status);
        Assert.NotNull(declaration.SubmittedAtUtc);
        Assert.Equal(userId, declaration.UpdatedBy);
    }

    [Fact]
    public void AssignEctnReference_StoresReference()
    {
        var declaration = NewDraft();

        declaration.AssignEctnReference("ECTN-123");

        Assert.Equal("ECTN-123", declaration.EctnReference);
    }

    [Fact]
    public void Accept_SetsAcceptedStatusAndDeclarationNumber()
    {
        var declaration = NewDraft();
        var userId = Guid.NewGuid();

        declaration.Accept("DEC-2026-0001", userId);

        Assert.Equal(DeclarationStatus.Accepted.ToString(), declaration.Status);
        Assert.Equal("DEC-2026-0001", declaration.DeclarationNumber);
        Assert.NotNull(declaration.AcceptedAtUtc);
        Assert.Equal(userId, declaration.UpdatedBy);
    }

    [Fact]
    public void Reject_SetsRejectedStatusAndTimestamp()
    {
        var declaration = NewDraft();

        declaration.Reject("R001", "Missing documents", Guid.NewGuid());

        Assert.Equal(DeclarationStatus.Rejected.ToString(), declaration.Status);
        Assert.NotNull(declaration.RejectedAtUtc);
    }

    private static CustomsDeclaration NewDraft() =>
        new(Guid.NewGuid(), "APP-REF-1", DeclarationType.Export, "PROC-2", Guid.NewGuid());
}
