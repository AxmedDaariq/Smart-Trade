using Customs.Domain.Entities;
using Customs.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CustomsValuationPolicy"/>.
/// </summary>
public sealed class CustomsValuationPolicyTests
{
    private readonly CustomsValuationPolicy _policy = new();

    [Fact]
    public void CalculateCustomsValue_TransactionValue_CalculatesCifWithAdditionsAndDeductions()
    {
        // Arrange: Invoice(10000) + Freight(1000) + Insurance(500) + Additions(200) - Deductions(100) = 11600
        var valuation = new CustomsValuation
        {
            ValuationMethod = "TransactionValue",
            InvoiceValue = 10000m,
            FreightValue = 1000m,
            InsuranceValue = 500m,
            Additions = 200m,
            Deductions = 100m,
            ExchangeRate = 1.0m
        };

        // Act
        var result = _policy.CalculateCustomsValue(valuation);

        // Assert
        result.Should().Be(11600m);
    }

    [Fact]
    public void CalculateCustomsValue_WithExchangeRate_AppliesRateCorrectly()
    {
        // Arrange: (1000 + 200 + 100) * 570 SOS = 741,000 SOS
        var valuation = new CustomsValuation
        {
            ValuationMethod = "TransactionValue",
            InvoiceValue = 1000m,
            FreightValue = 200m,
            InsuranceValue = 100m,
            Additions = 0m,
            Deductions = 0m,
            ExchangeRate = 570m
        };

        // Act
        var result = _policy.CalculateCustomsValue(valuation);

        // Assert
        result.Should().Be(741000m);
    }

    [Fact]
    public void CalculateCustomsValue_ZeroOrNegativeExchangeRate_DefaultsToOne()
    {
        // Arrange
        var valuation = new CustomsValuation
        {
            ValuationMethod = "TransactionValue",
            InvoiceValue = 5000m,
            FreightValue = 500m,
            InsuranceValue = 100m,
            ExchangeRate = 0m // Zero rate should fall back to 1.0
        };

        // Act
        var result = _policy.CalculateCustomsValue(valuation);

        // Assert
        result.Should().Be(5600m);
    }

    [Fact]
    public void CalculateCustomsValue_NegativeResult_ClampsToZero()
    {
        // Arrange: Deductions exceeding total value
        var valuation = new CustomsValuation
        {
            ValuationMethod = "TransactionValue",
            InvoiceValue = 100m,
            Deductions = 500m,
            ExchangeRate = 1.0m
        };

        // Act
        var result = _policy.CalculateCustomsValue(valuation);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void CalculateCustomsValue_FallbackMethod_UsesInvoiceValueMultipliedByRate()
    {
        // Arrange: Non-transaction method
        var valuation = new CustomsValuation
        {
            ValuationMethod = "ComputedValue",
            InvoiceValue = 4000m,
            FreightValue = 800m,
            ExchangeRate = 2.0m
        };

        // Act
        var result = _policy.CalculateCustomsValue(valuation);

        // Assert
        result.Should().Be(8000m);
    }
}
