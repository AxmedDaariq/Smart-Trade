using Customs.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Domain;

public class CustomsCaseTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId   = Guid.NewGuid();

    // ── Constructor sets defaults ─────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidArguments_SetsPropertiesCorrectly()
    {
        var customsCase = new CustomsCase(
            tenantId:             TenantId,
            declarationExternalId:"DECL-EXT-001",
            declarationNumber:    "DECL-2026-001",
            ectnExternalId:       "ECTN-EXT-001",
            ectnNumber:           "ECTN-2026-001",
            caseNumber:           "CASE-001",
            procedureCode:        "IM4",
            importerReference:    "IMP-REF",
            exporterReference:    "EXP-REF",
            declarantReference:   "DEC-REF",
            portOfEntry:          "MOG",
            portOfExit:           "JIB",
            countryOfOrigin:      "SO",
            countryOfDestination: "DJ",
            userId:               UserId);

        customsCase.TenantId.Should().Be(TenantId);
        customsCase.DeclarationExternalId.Should().Be("DECL-EXT-001");
        customsCase.DeclarationNumber.Should().Be("DECL-2026-001");
        customsCase.EctnNumber.Should().Be("ECTN-2026-001");
        customsCase.CaseNumber.Should().Be("CASE-001");
        customsCase.ProcedureCode.Should().Be("IM4");
        customsCase.ImporterReference.Should().Be("IMP-REF");
        customsCase.ExporterReference.Should().Be("EXP-REF");
        customsCase.DeclarantReference.Should().Be("DEC-REF");
        customsCase.PortOfEntry.Should().Be("MOG");
        customsCase.PortOfExit.Should().Be("JIB");
        customsCase.CountryOfOrigin.Should().Be("SO");
        customsCase.CountryOfDestination.Should().Be("DJ");
        customsCase.CreatedBy.Should().Be(UserId);
    }

    [Fact]
    public void Constructor_DefaultStatus_IsReceived()
    {
        var customsCase = BuildCase();
        customsCase.Status.Should().Be("Received");
    }

    [Fact]
    public void Constructor_OpenedAtUtc_IsSetToNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var customsCase = BuildCase();
        var after  = DateTime.UtcNow.AddSeconds(1);

        customsCase.OpenedAtUtc.Should().BeAfter(before).And.BeBefore(after);
        customsCase.CreatedAtUtc.Should().BeAfter(before).And.BeBefore(after);
    }

    // ── Default collections ───────────────────────────────────────────────────

    [Fact]
    public void Constructor_Collections_AreInitializedEmpty()
    {
        var customsCase = BuildCase();

        customsCase.Assessments.Should().BeEmpty();
        customsCase.Valuations.Should().BeEmpty();
        customsCase.Holds.Should().BeEmpty();
        customsCase.Discrepancies.Should().BeEmpty();
        customsCase.Decisions.Should().BeEmpty();
        customsCase.StatusHistories.Should().BeEmpty();
    }

    // ── Default CaseType ─────────────────────────────────────────────────────

    [Fact]
    public void DefaultCaseType_IsImport()
    {
        var customsCase = new CustomsCase();
        customsCase.CaseType.Should().Be("Import");
    }

    // ── Null-safe strings in constructor ─────────────────────────────────────

    [Fact]
    public void Constructor_NullStrings_DefaultToEmpty()
    {
        var customsCase = new CustomsCase(
            tenantId:             TenantId,
            declarationExternalId:null!,
            declarationNumber:    null!,
            ectnExternalId:       null,
            ectnNumber:           null,
            caseNumber:           null!,
            procedureCode:        null!,
            importerReference:    null!,
            exporterReference:    null!,
            declarantReference:   null!,
            portOfEntry:          null!,
            portOfExit:           null!,
            countryOfOrigin:      null!,
            countryOfDestination: null!,
            userId:               UserId);

        customsCase.DeclarationExternalId.Should().Be(string.Empty);
        customsCase.DeclarationNumber.Should().Be(string.Empty);
        customsCase.CaseNumber.Should().Be(string.Empty);
        customsCase.EctnNumber.Should().BeNull();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CustomsCase BuildCase() => new(
        tenantId:             TenantId,
        declarationExternalId:"DECL-EXT-001",
        declarationNumber:    "DECL-2026-001",
        ectnExternalId:       "ECTN-EXT-001",
        ectnNumber:           "ECTN-2026-001",
        caseNumber:           "CASE-001",
        procedureCode:        "IM4",
        importerReference:    "IMP-REF",
        exporterReference:    "EXP-REF",
        declarantReference:   "DEC-REF",
        portOfEntry:          "MOG",
        portOfExit:           "JIB",
        countryOfOrigin:      "SO",
        countryOfDestination: "DJ",
        userId:               UserId);
}
