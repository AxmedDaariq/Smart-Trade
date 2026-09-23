using System;
using Customs.Domain.Entities;
using Customs.Domain.Enums;

namespace Customs.Domain.Policies;

public interface IRiskRuleEngine
{
    RiskResult Evaluate(CustomsCase customsCase);
}

public class RiskResult
{
    public RiskLevel Level { get; set; } = RiskLevel.Low;
    public string[] MatchedRules { get; set; } = Array.Empty<string>();
    public string[] Reasons { get; set; } = Array.Empty<string>();
    public decimal RiskScore { get; set; }
}

public interface ICustomsValuationPolicy
{
    decimal CalculateCustomsValue(CustomsValuation valuation);
}

public class CustomsValuationPolicy : ICustomsValuationPolicy
{
    public decimal CalculateCustomsValue(CustomsValuation valuation)
    {
        var rate = valuation.ExchangeRate <= 0 ? 1m : valuation.ExchangeRate;

        if (string.Equals(valuation.ValuationMethod, nameof(ValuationMethod.TransactionValue), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(valuation.ValuationMethod, "TransactionValue", StringComparison.OrdinalIgnoreCase))
        {
            var cif = valuation.InvoiceValue + valuation.FreightValue + valuation.InsuranceValue + valuation.Additions - valuation.Deductions;
            return Math.Max(0, cif * rate);
        }
        
        // Default calculation for other methods until explicitly implemented
        return Math.Max(0, valuation.InvoiceValue * rate);
    }
}
