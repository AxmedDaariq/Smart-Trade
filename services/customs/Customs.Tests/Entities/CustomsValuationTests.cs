using System;
using Customs.Domain.Entities;
using Xunit;

namespace Customs.Tests.Entities;

public class CustomsValuationTests
{
    [Fact]
    public void CustomsValuation_Should_Initialize_With_Correct_Defaults()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var declarationId = Guid.NewGuid();
        var calculatedBy = Guid.NewGuid();

        // Act
        var valuation = new CustomsValuation
        {
            CustomsCaseId = caseId,
            DeclarationId = declarationId,
            ValuationMethod = "TransactionValue",
            InvoiceValue = 8000m,
            FreightValue = 300m,
            InsuranceValue = 100m,
            Additions = 50m,
            Deductions = 0m,
            ExchangeRate = 1.0m,
            ConvertedValue = 8450m,
            CurrencyCode = "USD",
            BaseCurrencyCode = "USD",
            CalculatedBy = calculatedBy
        };

        // Assert
        Assert.NotEqual(Guid.Empty, valuation.Id);
        Assert.Equal(caseId, valuation.CustomsCaseId);
        Assert.Equal("TransactionValue", valuation.ValuationMethod);
        Assert.Equal("Calculated", valuation.Status);
        Assert.Equal(8000m, valuation.InvoiceValue);
        Assert.Equal(300m, valuation.FreightValue);
        Assert.Equal(100m, valuation.InsuranceValue);
        Assert.Equal(8450m, valuation.ConvertedValue);
        Assert.Equal("USD", valuation.CurrencyCode);
        Assert.Equal("USD", valuation.BaseCurrencyCode);
        
        // Collections should be initialized and empty
        Assert.NotNull(valuation.Components);
        Assert.Empty(valuation.Components);
    }

    [Fact]
    public void CustomsValuation_CIF_Value_Should_Equal_Sum_Of_Invoice_Freight_Insurance()
    {
        // Arrange - CIF = Cost + Insurance + Freight
        var invoice  = 10000m;
        var freight  = 500m;
        var insurance = 200m;
        var expectedCIF = invoice + freight + insurance; // 10700m

        // Act
        var valuation = new CustomsValuation
        {
            InvoiceValue  = invoice,
            FreightValue  = freight,
            InsuranceValue = insurance,
            ConvertedValue = expectedCIF,
            ExchangeRate = 1.0m
        };

        // Assert
        Assert.Equal(expectedCIF, valuation.ConvertedValue);
        Assert.Equal(10700m, valuation.ConvertedValue);
    }

    [Fact]
    public void CustomsValuation_With_ExchangeRate_Should_Convert_Correctly()
    {
        // Arrange - USD to SOS (Somalia Shilling) conversion
        var invoiceUSD   = 5000m;
        var exchangeRate = 570m;  // 1 USD = 570 SOS
        var expectedConverted = invoiceUSD * exchangeRate; // 2,850,000 SOS

        // Act
        var valuation = new CustomsValuation
        {
            InvoiceValue    = invoiceUSD,
            ExchangeRate    = exchangeRate,
            ConvertedValue  = invoiceUSD * exchangeRate,
            CurrencyCode    = "USD",
            BaseCurrencyCode = "SOS"
        };

        // Assert
        Assert.Equal(expectedConverted, valuation.ConvertedValue);
        Assert.Equal("USD", valuation.CurrencyCode);
        Assert.Equal("SOS", valuation.BaseCurrencyCode);
    }
}
