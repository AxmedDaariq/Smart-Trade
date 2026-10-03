using System;
using Integration.Domain.Entities;

namespace Integration.Tests.Domain.Entities;

public class WebhookInboundEventTests
{
    [Fact]
    public void NewWebhookInboundEvent_ShouldDefaultToReceivedState()
    {
        // Arrange & Act
        var webhook = new WebhookInboundEvent();

        // Assert
        Assert.Equal("Received", webhook.Status);
        Assert.False(webhook.SignatureVerified);
        Assert.Null(webhook.ProcessedAtUtc);
        Assert.Null(webhook.ErrorCode);
        Assert.Null(webhook.OccurredAtUtc);
        Assert.Null(webhook.TenantId);
        Assert.Equal(string.Empty, webhook.PayloadHash);
        Assert.Equal(string.Empty, webhook.EventType);
    }

    [Fact]
    public void NewWebhookInboundEvent_ShouldHaveUniqueIdAndTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var webhook = new WebhookInboundEvent();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, webhook.Id);
        Assert.InRange(webhook.ReceivedAtUtc, before, after);
        Assert.InRange(webhook.CreatedAtUtc, before, after);
    }

    [Fact]
    public void WebhookInboundEvent_Lifecycle_ShouldBeTrackable()
    {
        // Arrange
        var webhook = new WebhookInboundEvent
        {
            PartnerId = Guid.NewGuid(),
            ProviderEventId = "evt-12345",
            EventType = "vessel.arrived",
            PayloadHash = "sha256:abc"
        };

        // Act: verification succeeds and the processor finishes the event
        webhook.SignatureVerified = true;
        webhook.Status = "Processed";
        webhook.ProcessedAtUtc = DateTime.UtcNow;

        // Assert
        Assert.True(webhook.SignatureVerified);
        Assert.Equal("Processed", webhook.Status);
        Assert.NotNull(webhook.ProcessedAtUtc);
        Assert.Equal("evt-12345", webhook.ProviderEventId);
        Assert.NotEqual(Guid.Empty, webhook.PartnerId);
    }

    [Fact]
    public void WebhookInboundEvent_RejectedState_ShouldCarryErrorCode()
    {
        // Arrange
        var webhook = new WebhookInboundEvent();

        // Act
        webhook.Status = "Rejected";
        webhook.SignatureVerified = false;
        webhook.ErrorCode = "SIGNATURE_MISMATCH";

        // Assert
        Assert.Equal("Rejected", webhook.Status);
        Assert.Equal("SIGNATURE_MISMATCH", webhook.ErrorCode);
    }
}
