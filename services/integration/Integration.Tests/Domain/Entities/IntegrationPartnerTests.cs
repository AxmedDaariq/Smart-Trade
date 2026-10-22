using System;
using Integration.Domain.Entities;

namespace Integration.Tests.Domain.Entities;

public class IntegrationPartnerTests
{
    [Fact]
    public void NewPartner_ShouldDefaultToDraftWithStandardTrust()
    {
        // Arrange & Act
        var partner = new IntegrationPartner();

        // Assert: partners start as drafts until they are activated
        Assert.Equal("Draft", partner.Status);
        Assert.Equal("Standard", partner.TrustLevel);
        Assert.Null(partner.TenantId);
        Assert.Null(partner.UpdatedBy);
        Assert.Empty(partner.Version);
        Assert.Empty(partner.Systems);
    }

    [Fact]
    public void NewPartner_ShouldHaveUniqueIdAndTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var partner = new IntegrationPartner();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, partner.Id);
        Assert.InRange(partner.CreatedAtUtc, before, after);
        Assert.InRange(partner.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewPartner_ShouldAllowLinkingSystems()
    {
        // Arrange
        var partner = new IntegrationPartner
        {
            PartnerReference = "PTR-001",
            Name = "Djibouti Customs",
            PartnerType = "CustomsSystem",
            CountryCode = "DJ"
        };
        var system = new IntegrationSystem
        {
            SystemReference = "SYS-001",
            Name = "ASYCUDA World"
        };

        // Act
        partner.Systems.Add(system);

        // Assert
        Assert.Single(partner.Systems);
        Assert.Same(system, partner.Systems.First());
        Assert.Equal("PTR-001", partner.PartnerReference);
        Assert.Equal("SYS-001", partner.Systems.First().SystemReference);
    }

    [Fact]
    public void PartnerProperties_ShouldBeSettable()
    {
        // Arrange
        var partner = new IntegrationPartner();
        var createdBy = Guid.NewGuid();

        // Act
        partner.CreatedBy = createdBy;
        partner.Status = "Active";
        partner.TrustLevel = "Trusted";
        partner.Description = "National customs authority";

        // Assert
        Assert.Equal(createdBy, partner.CreatedBy);
        Assert.Equal("Active", partner.Status);
        Assert.Equal("Trusted", partner.TrustLevel);
        Assert.Equal("National customs authority", partner.Description);
    }
}

