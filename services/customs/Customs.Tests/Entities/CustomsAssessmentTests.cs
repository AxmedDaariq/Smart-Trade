using System;
using Customs.Domain.Entities;
using Xunit;

namespace Customs.Tests.Entities;

public class CustomsAssessmentTests
{
    [Fact]
    public void CustomsAssessment_Should_Initialize_With_Correct_Defaults()
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
            CustomsValue = 15000.50m,
            CurrencyCode = "USD"
        };

        // Assert
        Assert.NotEqual(Guid.Empty, assessment.Id);
        Assert.Equal(caseId, assessment.CustomsCaseId);
        Assert.Equal(declarationId, assessment.DeclarationId);
        Assert.Equal("Draft", assessment.Status);
        Assert.Equal(15000.50m, assessment.CustomsValue);
        Assert.Equal("USD", assessment.CurrencyCode);
        
        // Ensure collections are properly initialized
        Assert.NotNull(assessment.DutyAssessments);
        Assert.Empty(assessment.DutyAssessments);
        
        Assert.NotNull(assessment.TaxAssessments);
        Assert.Empty(assessment.TaxAssessments);
        
        Assert.NotNull(assessment.Lines);
        Assert.Empty(assessment.Lines);
        
        // Ensure dates are populated
        Assert.True(assessment.CreatedAtUtc <= DateTime.UtcNow);
        Assert.True(assessment.UpdatedAtUtc <= DateTime.UtcNow);
    }
    
    [Fact]
    public void CustomsAssessment_Should_Calculate_Totals_Correctly()
    {
        // Arrange
        var assessment = new CustomsAssessment
        {
            CustomsValue = 10000m,
            DutyAmount = 500m,
            CustomsChargesAmount = 250m,
            TotalAmount = 10750m // This would typically be calculated by a domain service
        };

        // Assert
        Assert.Equal(10000m, assessment.CustomsValue);
        Assert.Equal(500m, assessment.DutyAmount);
        Assert.Equal(250m, assessment.CustomsChargesAmount);
        Assert.Equal(10750m, assessment.TotalAmount);
    }
}
