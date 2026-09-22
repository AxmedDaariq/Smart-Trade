using System;

namespace Payment.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    protected Money() { }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency must be specified", nameof(currency));

        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero); // Default rounding policy for now
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency) => new Money(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");
        if (Amount < other.Amount)
            throw new InvalidOperationException("Cannot subtract to a negative amount");
        return new Money(Amount - other.Amount, Currency);
    }

    public bool Equals(Money other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}
