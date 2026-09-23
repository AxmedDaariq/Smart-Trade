using Customs.Domain.Entities;
using Customs.Domain.Enums;
using Customs.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Domain;

public class CustomsValuationPolicyTests
{
    private readonly CustomsValuationPolicy _policy = new();

    // ── TransactionValue method ───────────────────────────────────────────────

    [Fact]
    public void CalculateCustomsValue_TransactionValue_CIF_NoExchange_ReturnsCorrectValue()
    {
        // CIF = invoice + freight + insurance + additions - deductions
        // 1000 + 150 + 50 + 0 - 0 = 1200, exchange=1 => 1200
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 1000m,
            freightValue: 150m,
            insuranceValue: 50m,
            additions: 0m,
            deductions: 0m,
            exchangeRate: 1m);

        _policy.CalculateCustomsValue(valuation).Should().Be(1200m);
    }

    [Fact]
    public void CalculateCustomsValue_TransactionValue_WithExchangeRate_ConvertsCorrectly()
    {
        // CIF = 1000, exchange=3.65 => 3650
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 1000m,
            freightValue: 0m,
            insuranceValue: 0m,
            additions: 0m,
            deductions: 0m,
            exchangeRate: 3.65m);

        _policy.CalculateCustomsValue(valuation).Should().Be(3650m);
    }

    [Fact]
    public void CalculateCustomsValue_TransactionValue_WithAdditionsAndDeductions()
    {
        // 500 + 100 + 25 + 75 - 50 = 650, rate=1
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 500m,
            freightValue: 100m,
            insuranceValue: 25m,
            additions: 75m,
            deductions: 50m,
            exchangeRate: 1m);

        _policy.CalculateCustomsValue(valuation).Should().Be(650m);
    }

    [Fact]
    public void CalculateCustomsValue_TransactionValue_NegativeCIF_ReturnsZero()
    {
        // Math.Max(0, negative) => 0
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 100m,
            freightValue: 0m,
            insuranceValue: 0m,
            additions: 0m,
            deductions: 500m,
            exchangeRate: 1m);

        _policy.CalculateCustomsValue(valuation).Should().Be(0m);
    }

    // ── Zero / invalid exchange rates ─────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void CalculateCustomsValue_ZeroOrNegativeExchangeRate_TreatedAsOne(decimal badRate)
    {
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 1000m,
            freightValue: 0m,
            insuranceValue: 0m,
            additions: 0m,
            deductions: 0m,
            exchangeRate: badRate);

        // exchange defaults to 1 => result = 1000
        _policy.CalculateCustomsValue(valuation).Should().Be(1000m);
    }

    // ── Fallback (non-TransactionValue methods) ───────────────────────────────

    [Theory]
    [InlineData("IdenticalGoods")]
    [InlineData("SimilarGoods")]
    [InlineData("DeductiveValue")]
    [InlineData("ComputedValue")]
    [InlineData("FallbackMethod")]
    public void CalculateCustomsValue_NonTransactionValueMethod_UsesInvoiceValueOnly(string method)
    {
        var valuation = BuildValuation(
            method: method,
            invoiceValue: 800m,
            freightValue: 200m,   // ignored in fallback
            insuranceValue: 100m, // ignored in fallback
            additions: 50m,       // ignored in fallback
            deductions: 0m,
            exchangeRate: 2m);

        // fallback = invoiceValue * rate = 800 * 2 = 1600
        _policy.CalculateCustomsValue(valuation).Should().Be(1600m);
    }

    // ── Edge: zero invoice ────────────────────────────────────────────────────

    [Fact]
    public void CalculateCustomsValue_ZeroInvoice_ReturnsZero()
    {
        var valuation = BuildValuation(
            method: "TransactionValue",
            invoiceValue: 0m,
            freightValue: 0m,
            insuranceValue: 0m,
            additions: 0m,
            deductions: 0m,
            exchangeRate: 3m);

        _policy.CalculateCustomsValue(valuation).Should().Be(0m);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CustomsValuation BuildValuation(
        string method,
        decimal invoiceValue,
        decimal freightValue,
        decimal insuranceValue,
        decimal additions,
        decimal deductions,
        decimal exchangeRate) => new()
    {
        ValuationMethod = method,
        InvoiceValue    = invoiceValue,
        FreightValue    = freightValue,
        InsuranceValue  = insuranceValue,
        Additions       = additions,
        Deductions      = deductions,
        ExchangeRate    = exchangeRate,
        CurrencyCode    = "USD"
    };
}
