using Corridor.Domain.Entities;
using Corridor.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Corridor.UnitTests.Domain;

public class CorridorEntityTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    // ── Defaults ──────────────────────────────────────────────────────────────

    [Fact]
    public void Corridor_DefaultStatus_IsDraft()
    {
        var corridor = BuildCorridor();
        corridor.Status.Should().Be("Draft");
    }

    [Fact]
    public void Corridor_DefaultVersion_IsOne()
    {
        var corridor = BuildCorridor();
        corridor.CurrentVersion.Should().Be(1);
    }

    [Fact]
    public void Corridor_AutoGeneratesNonEmptyId()
    {
        var corridor = BuildCorridor();
        corridor.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Corridor_Versions_DefaultIsEmpty()
    {
        var corridor = BuildCorridor();
        corridor.Versions.Should().BeEmpty();
    }

    // ── Status transitions ────────────────────────────────────────────────────

    [Theory]
    [InlineData("Draft")]
    [InlineData("Active")]
    [InlineData("Suspended")]
    [InlineData("Inactive")]
    [InlineData("Archived")]
    public void Corridor_Status_AcceptsAllValidValues(string status)
    {
        var corridor = BuildCorridor();
        corridor.Status = status;
        corridor.Status.Should().Be(status);
    }

    // ── OriginLocation / Destination ─────────────────────────────────────────

    [Fact]
    public void Corridor_CanSetOriginAndDestination()
    {
        var corridor = BuildCorridor();
        corridor.OriginLocation.Should().Be("Mogadishu Port");
        corridor.DestinationLocation.Should().Be("Nairobi ICD");
    }

    // ── CreatedBy / UpdatedBy ─────────────────────────────────────────────────

    [Fact]
    public void Corridor_CreatedBy_IsSet()
    {
        var userId   = Guid.NewGuid();
        var corridor = BuildCorridor(createdBy: userId);
        corridor.CreatedBy.Should().Be(userId);
    }

    [Fact]
    public void Corridor_UpdatedBy_DefaultIsNull()
    {
        var corridor = BuildCorridor();
        corridor.UpdatedBy.Should().BeNull();
    }

    // ── CorridorStatus enum coverage ─────────────────────────────────────────

    [Fact]
    public void CorridorStatus_Enum_HasExpectedValues()
    {
        var values = Enum.GetNames(typeof(CorridorStatus));
        values.Should().Contain(new[] { "Draft", "Active", "Suspended", "Inactive", "Archived" });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Corridor.Domain.Entities.Corridor BuildCorridor(Guid? createdBy = null) => new()
    {
        TenantId             = TenantId,
        CorridorReference    = "CORR-2026-001",
        Name                 = "Mogadishu–Nairobi Corridor",
        OriginLocation       = "Mogadishu Port",
        DestinationLocation  = "Nairobi ICD",
        Status               = "Draft",
        CreatedBy            = createdBy ?? Guid.NewGuid()
    };
}
