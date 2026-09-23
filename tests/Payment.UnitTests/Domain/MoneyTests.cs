using Payment.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Payment.UnitTests.Domain;

public class MoneyTests
{
    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        var money = new Money(100.50m, "usd");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD"); // normalized to upper-case
    }

    [Fact]
    public void Constructor_AmountIsRoundedToTwoDecimalPlaces()
    {
        var money = new Money(100.555m, "USD");
        money.Amount.Should().Be(100.56m); // MidpointRounding.AwayFromZero
    }

    [Fact]
    public void Constructor_NegativeAmount_Throws()
    {
        var act = () => new Money(-1m, "USD");
        act.Should().Throw<ArgumentException>().WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Constructor_EmptyCurrency_Throws()
    {
        var act = () => new Money(100m, "");
        act.Should().Throw<ArgumentException>().WithMessage("*Currency must be specified*");
    }

    [Fact]
    public void Constructor_NullCurrency_Throws()
    {
        var act = () => new Money(100m, null!);
        act.Should().Throw<ArgumentException>().WithMessage("*Currency must be specified*");
    }

    [Fact]
    public void Constructor_WhitespaceCurrency_Throws()
    {
        var act = () => new Money(100m, "   ");
        act.Should().Throw<ArgumentException>().WithMessage("*Currency must be specified*");
    }

    // ── Zero factory ─────────────────────────────────────────────────────────

    [Fact]
    public void Zero_ReturnsMoneyWithZeroAmount()
    {
        var zero = Money.Zero("USD");
        zero.Amount.Should().Be(0m);
        zero.Currency.Should().Be("USD");
    }

    // ── Add ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "USD");

        var result = a.Add(b);

        result.Amount.Should().Be(300m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_DifferentCurrencies_Throws()
    {
        var usd = new Money(100m, "USD");
        var eur = new Money(100m, "EUR");

        var act = () => usd.Add(eur);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot add different currencies*");
    }

    // ── Subtract ──────────────────────────────────────────────────────────────

    [Fact]
    public void Subtract_SameCurrency_ReturnsDifference()
    {
        var a = new Money(500m, "USD");
        var b = new Money(200m, "USD");

        var result = a.Subtract(b);

        result.Amount.Should().Be(300m);
    }

    [Fact]
    public void Subtract_DifferentCurrencies_Throws()
    {
        var usd = new Money(100m, "USD");
        var eur = new Money(50m, "EUR");

        var act = () => usd.Subtract(eur);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot subtract different currencies*");
    }

    [Fact]
    public void Subtract_ResultWouldBeNegative_Throws()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "USD");

        var act = () => a.Subtract(b);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot subtract to a negative amount*");
    }

    // ── Equality ──────────────────────────────────────────────────────────────

    [Fact]
    public void Equals_SameAmountAndCurrency_ReturnsTrue()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "USD");

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        var a = new Money(100m, "USD");
        var b = new Money(200m, "USD");

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentCurrency_ReturnsFalse()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "EUR");

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var a = new Money(100m, "USD");
        a.Equals(null).Should().BeFalse();
    }

    // ── GetHashCode ───────────────────────────────────────────────────────────

    [Fact]
    public void GetHashCode_EqualObjects_ReturnSameHash()
    {
        var a = new Money(100m, "USD");
        var b = new Money(100m, "USD");

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    // ── Zero amount is valid ──────────────────────────────────────────────────

    [Fact]
    public void Constructor_ZeroAmount_IsValid()
    {
        var act = () => new Money(0m, "USD");
        act.Should().NotThrow();
    }
}
