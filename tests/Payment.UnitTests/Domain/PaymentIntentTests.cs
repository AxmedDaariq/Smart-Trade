using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Payment.UnitTests.Domain;

public class PaymentIntentTests
{
    private static readonly Guid TenantId       = Guid.NewGuid();
    private static readonly Guid CustomsCaseId  = Guid.NewGuid();

    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidArguments_SetsPropertiesCorrectly()
    {
        var money = new Money(5000m, "USD");
        var intent = new PaymentIntent(
            tenantId:          TenantId,
            intentReference:   "PI-REF-001",
            sourceSystem:      "Customs",
            sourceReferenceId: "ASSESS-001",
            customsCaseId:     CustomsCaseId,
            declarationNumber: "DECL-001",
            ectnNumber:        "ECTN-001",
            money:             money);

        intent.TenantId.Should().Be(TenantId);
        intent.IntentReference.Should().Be("PI-REF-001");
        intent.SourceSystem.Should().Be("Customs");
        intent.SourceReferenceId.Should().Be("ASSESS-001");
        intent.CustomsCaseId.Should().Be(CustomsCaseId);
        intent.DeclarationNumber.Should().Be("DECL-001");
        intent.ECTNNumber.Should().Be("ECTN-001");
        intent.AmountRequested.Should().Be(5000m);
        intent.CurrencyCode.Should().Be("USD");
        intent.Status.Should().Be("Created");
    }

    [Fact]
    public void Constructor_DefaultStatus_IsCreated()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Status.Should().Be("Created");
    }

    [Fact]
    public void Constructor_WithDueDate_SetsDueDate()
    {
        var dueDate = DateTime.UtcNow.AddDays(30);
        var intent = new PaymentIntent(
            TenantId, "REF", "Customs", "SRC", CustomsCaseId,
            "DECL", "ECTN", new Money(100m, "USD"), dueDate);

        intent.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
    }

    // ── Process ───────────────────────────────────────────────────────────────

    [Fact]
    public void Process_FromCreated_SetsStatusToProcessing()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Process();
        intent.Status.Should().Be("Processing");
    }

    [Fact]
    public void Process_FromRequiresPaymentMethod_SetsStatusToProcessing()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Status = "RequiresPaymentMethod";
        intent.Process();
        intent.Status.Should().Be("Processing");
    }

    [Fact]
    public void Process_FromInvalidState_Throws()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Status = "Succeeded";

        var act = () => intent.Process();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot process payment intent*");
    }

    // ── MarkSucceeded ─────────────────────────────────────────────────────────

    [Fact]
    public void MarkSucceeded_FromProcessing_SetsStatusToSucceeded()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Process();
        var tx = BuildTransaction(intent.Id);

        intent.MarkSucceeded(tx);

        intent.Status.Should().Be("Succeeded");
        intent.Transactions.Should().ContainSingle();
    }

    [Fact]
    public void MarkSucceeded_AlreadySucceeded_IsIdempotent()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Process();
        intent.MarkSucceeded(BuildTransaction(intent.Id));

        // calling again should not throw
        var act = () => intent.MarkSucceeded(BuildTransaction(intent.Id));
        act.Should().NotThrow();
        intent.Status.Should().Be("Succeeded");
    }

    [Fact]
    public void MarkSucceeded_FromCanceled_Throws()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Cancel();

        var act = () => intent.MarkSucceeded(BuildTransaction(intent.Id));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot succeed a canceled*");
    }

    // ── MarkFailed ────────────────────────────────────────────────────────────

    [Fact]
    public void MarkFailed_SetsStatusToFailed()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Process();
        intent.MarkFailed("Gateway timeout");

        intent.Status.Should().Be("Failed");
    }

    // ── Cancel ────────────────────────────────────────────────────────────────

    [Fact]
    public void Cancel_FromCreated_SetsStatusToCanceled()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Cancel();
        intent.Status.Should().Be("Canceled");
    }

    [Fact]
    public void Cancel_FromSucceeded_Throws()
    {
        var intent = BuildIntent(1000m, "USD");
        intent.Process();
        intent.MarkSucceeded(BuildTransaction(intent.Id));

        var act = () => intent.Cancel();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Cannot cancel an already succeeded*");
    }

    // ── Customs <-> Payment link ──────────────────────────────────────────────

    [Fact]
    public void PaymentIntent_LinkedToCustomsCase_StoresCorrectCaseId()
    {
        var caseId = Guid.NewGuid();
        var intent = new PaymentIntent(
            TenantId, "PI-2026-001", "Customs", "ASSESS-XYZ", caseId,
            "DECL-2026-001", "ECTN-2026-001", new Money(12500m, "USD"));

        intent.CustomsCaseId.Should().Be(caseId);
        intent.SourceSystem.Should().Be("Customs");
        intent.SourceReferenceId.Should().Be("ASSESS-XYZ");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static PaymentIntent BuildIntent(decimal amount, string currency)
        => new(TenantId, "PI-REF", "Customs", "ASSESS-001", CustomsCaseId,
               "DECL-001", "ECTN-001", new Money(amount, currency));

    private static PaymentTransaction BuildTransaction(Guid intentId)
        => new() { PaymentIntentId = intentId, Amount = 1000m, CurrencyCode = "USD" };
}
