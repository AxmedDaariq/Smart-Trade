using System;
using Customs.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Domain;

/// <summary>
/// Unit tests for domain entities in Customs.
/// </summary>
public sealed class CustomsDomainTests
{
    [Fact]
    public void CustomsCase_Constructor_InitializesAllPropertiesAndCollections()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var c = new CustomsCase(
            tenantId: tenantId,
            declarationExternalId: "DEC-EXT-999",
            declarationNumber: "DEC-NUM-100",
            ectnExternalId: "ECTN-EXT-200",
            ectnNumber: "ECTN-NUM-300",
            caseNumber: "CC-2026-TEST-001",
            procedureCode: "IM4",
            importerReference: "IMP-SOM",
            exporterReference: "EXP-UAE",
            declarantReference: "DEC-AGT",
            portOfEntry: "BERBERA",
            portOfExit: "JEBEL_ALI",
            countryOfOrigin: "AE",
            countryOfDestination: "SO",
            userId: userId
        );

        // Assert
        c.Id.Should().NotBeEmpty();
        c.TenantId.Should().Be(tenantId);
        c.CreatedBy.Should().Be(userId);
        c.CaseNumber.Should().Be("CC-2026-TEST-001");
        c.DeclarationNumber.Should().Be("DEC-NUM-100");
        c.ECTNExternalId.Should().Be("ECTN-EXT-200");
        c.EctnNumber.Should().Be("ECTN-NUM-300");
        c.ProcedureCode.Should().Be("IM4");
        c.Status.Should().Be("Received");
        c.CaseType.Should().Be("Import");
        c.PortOfEntry.Should().Be("BERBERA");
        c.PortOfExit.Should().Be("JEBEL_ALI");
        c.CountryOfOrigin.Should().Be("AE");
        c.OpenedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(2));

        c.Holds.Should().NotBeNull().And.BeEmpty();
        c.Discrepancies.Should().NotBeNull().And.BeEmpty();
        c.Assessments.Should().NotBeNull().And.BeEmpty();
        c.Valuations.Should().NotBeNull().And.BeEmpty();
        c.Decisions.Should().NotBeNull().And.BeEmpty();
        c.StatusHistories.Should().NotBeNull().And.BeEmpty();
        c.References.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CustomsAssessment_Initialization_SetsDefaultsAndCalculations()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var declarationId = Guid.NewGuid();

        // Act
        var assessment = new CustomsAssessment
        {
            CustomsCaseId = caseId,
            DeclarationId = declarationId,
            AssessmentNumber = "ASM-2026-001",
            CustomsValue = 25000m,
            DutyAmount = 1250m,
            CustomsChargesAmount = 100m,
            TotalAmount = 26350m,
            CurrencyCode = "USD"
        };

        // Assert
        assessment.Id.Should().NotBeEmpty();
        assessment.CustomsCaseId.Should().Be(caseId);
        assessment.DeclarationId.Should().Be(declarationId);
        assessment.Status.Should().Be("Draft");
        assessment.CustomsValue.Should().Be(25000m);
        assessment.DutyAmount.Should().Be(1250m);
        assessment.CustomsChargesAmount.Should().Be(100m);
        assessment.TotalAmount.Should().Be(26350m);
        assessment.CurrencyCode.Should().Be("USD");
        assessment.DutyAssessments.Should().NotBeNull().And.BeEmpty();
        assessment.TaxAssessments.Should().NotBeNull().And.BeEmpty();
        assessment.Lines.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CustomsValuation_Initialization_SetsDefaultsAndCalculatesConvertedValue()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var declarationId = Guid.NewGuid();

        // Act
        var valuation = new CustomsValuation
        {
            CustomsCaseId = caseId,
            DeclarationId = declarationId,
            ValuationMethod = "TransactionValue",
            InvoiceValue = 10000m,
            FreightValue = 1500m,
            InsuranceValue = 500m,
            Additions = 200m,
            Deductions = 100m,
            ExchangeRate = 1.0m,
            ConvertedValue = 12100m,
            CurrencyCode = "USD",
            BaseCurrencyCode = "USD"
        };

        // Assert
        valuation.Id.Should().NotBeEmpty();
        valuation.CustomsCaseId.Should().Be(caseId);
        valuation.Status.Should().Be("Calculated");
        valuation.ValuationMethod.Should().Be("TransactionValue");
        valuation.InvoiceValue.Should().Be(10000m);
        valuation.ConvertedValue.Should().Be(12100m);
        valuation.Components.Should().NotBeNull().And.BeEmpty();
    }
}
