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
    public RiskLevel Level { get; set; }
    public string[] MatchedRules { get; set; }
    public string[] Reasons { get; set; }
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
        if (valuation.Method == ValuationMethod.TransactionValue)
        {
            return (valuation.InvoiceValue + valuation.Freight + valuation.Insurance + valuation.OtherAdditions - valuation.OtherDeductions) * valuation.ExchangeRate;
        }
        
        // Default calculation for other methods until explicitly implemented
        return valuation.InvoiceValue * valuation.ExchangeRate;
    }
}
