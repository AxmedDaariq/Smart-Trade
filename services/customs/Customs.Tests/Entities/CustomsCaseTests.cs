using System;
using Customs.Domain.Entities;
using Xunit;

namespace Customs.Tests.Entities;

public class CustomsCaseTests
{
    [Fact]
    public void CustomsCase_Should_Initialize_Correctly_With_Constructor()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        // Act
        var customsCase = new CustomsCase(
            tenantId: tenantId,
            declarationExternalId: "DEC-2026-001",
            declarationNumber: "1000123",
            ectnExternalId: "ECTN-XYZ",
            ectnNumber: "ECTN-2026-GEN-1234",
            caseNumber: "CASE-2026-999",
            procedureCode: "IM4",
            importerReference: "IMP-001",
            exporterReference: "EXP-002",
            declarantReference: "DCL-003",
            portOfEntry: "GEN",
            portOfExit: "SHA",
            countryOfOrigin: "CN",
            countryOfDestination: "SO",
            userId: userId
        );

        // Assert
        Assert.NotEqual(Guid.Empty, customsCase.Id);
        Assert.Equal(tenantId, customsCase.TenantId);
        Assert.Equal("Received", customsCase.Status);
        Assert.Equal("CASE-2026-999", customsCase.CaseNumber);
        Assert.Equal("1000123", customsCase.DeclarationNumber);
        Assert.Equal(userId, customsCase.CreatedBy);
        
        // Ensure collections are initialized
        Assert.NotNull(customsCase.Holds);
        Assert.NotNull(customsCase.Discrepancies);
        Assert.NotNull(customsCase.Assessments);
        Assert.NotNull(customsCase.StatusHistories);
    }
}
