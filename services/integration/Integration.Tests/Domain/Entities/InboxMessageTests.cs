using System;
using Integration.Domain.Entities;

namespace Integration.Tests.Domain.Entities;

public class InboxMessageTests
{
    [Fact]
    public void NewInboxMessage_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var first = new InboxMessage();
        var second = new InboxMessage();

        // Assert
        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void NewInboxMessage_ShouldDefaultToPendingStatus()
    {
        // Arrange & Act
        var message = new InboxMessage();

        // Assert: the message waits for a consumer to pick it up
        Assert.Equal("Pending", message.Status);
        Assert.Equal(0, message.RetryCount);
        Assert.Null(message.ProcessedAtUtc);
        Assert.Null(message.LastError);
        Assert.Equal(string.Empty, message.PayloadHash);
        Assert.Null(message.TenantId);
    }

    [Fact]
    public void NewInboxMessage_ShouldStampReceivedAndCreatedAtUtc()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var message = new InboxMessage();
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(message.ReceivedAtUtc, before, after);
        Assert.InRange(message.CreatedAtUtc, before, after);
    }

    [Fact]
    public void InboxMessage_Properties_ShouldBeSettable()
    {
        // Arrange
        var message = new InboxMessage();

        // Act
        message.Status = "Processed";
        message.RetryCount = 3;
        message.LastError = "consumer timeout";
        message.ProcessedAtUtc = DateTime.UtcNow;
        message.Consumer = "ectn-consumer";
        message.MessageType = "DeclarationSubmittedIntegrationEvent";

        // Assert
        Assert.Equal("Processed", message.Status);
        Assert.Equal(3, message.RetryCount);
        Assert.Equal("consumer timeout", message.LastError);
        Assert.NotNull(message.ProcessedAtUtc);
        Assert.Equal("ectn-consumer", message.Consumer);
        Assert.Equal("DeclarationSubmittedIntegrationEvent", message.MessageType);
    }
}
