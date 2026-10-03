using System;
using Integration.Domain.Entities;

namespace Integration.Tests.Domain.Entities;

public class WebhookSubscriptionTests
{
    [Fact]
    public void NewSubscription_ShouldDefaultToActiveWithHmacSha256()
    {
        // Arrange & Act
        var subscription = new WebhookSubscription();

        // Assert
        Assert.Equal("Active", subscription.Status);
        Assert.Equal("HMAC_SHA256", subscription.SignatureAlgorithm);
        Assert.Empty(subscription.Version);
        Assert.Empty(subscription.Deliveries);
    }

    [Fact]
    public void NewSubscription_ShouldHaveUniqueIdAndTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var subscription = new WebhookSubscription();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, subscription.Id);
        Assert.InRange(subscription.CreatedAtUtc, before, after);
        Assert.InRange(subscription.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void Subscription_Properties_ShouldBeSettable()
    {
        // Arrange
        var subscription = new WebhookSubscription
        {
            TenantId = Guid.NewGuid(),
            PartnerId = Guid.NewGuid(),
            WebhookReference = "WHK-001",
            Name = "Vessel arrival events",
            TargetUrl = "https://api.smart-trade.local/api/v1/integration/webhooks/PTR-001",
            EventType = "vessel.arrived",
            SecretReference = "vault/webhooks/PTR-001"
        };

        // Act
        subscription.Status = "Suspended";

        // Assert
        Assert.Equal("Suspended", subscription.Status);
        Assert.Equal("WHK-001", subscription.WebhookReference);
        Assert.StartsWith("https://", subscription.TargetUrl);
        Assert.NotEqual(Guid.Empty, subscription.TenantId);
        Assert.NotEqual(Guid.Empty, subscription.PartnerId);
    }
}
