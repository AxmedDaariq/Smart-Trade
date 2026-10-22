using System;
using Integration.Domain.Entities;

namespace Integration.Tests.Domain.Entities;

public class OutboxMessageTests
{
    [Fact]
    public void NewOutboxMessage_ShouldHaveUniqueId()
    {
        // Arrange & Act
        var first = new OutboxMessage();
        var second = new OutboxMessage();

        // Assert
        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void NewOutboxMessage_ShouldDefaultToPendingState()
    {
        // Arrange & Act
        var message = new OutboxMessage();

        // Assert: the message waits for the dispatcher to publish it
        Assert.Equal("Pending", message.Status);
        Assert.Equal("1.0", message.SchemaVersion);
        Assert.Equal(0, message.RetryCount);
        Assert.Null(message.LastAttemptAtUtc);
        Assert.Null(message.PublishedAtUtc);
        Assert.Null(message.ErrorMessage);
        Assert.Null(message.TenantId);
        Assert.Equal(string.Empty, message.PayloadJson);
    }

    [Fact]
    public void NewOutboxMessage_ShouldStampOccurredAtUtc()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var message = new OutboxMessage();
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(message.OccurredAtUtc, before, after);
        Assert.Equal(DateTimeKind.Utc, message.OccurredAtUtc.Kind);
    }

    [Fact]
    public void OutboxMessage_Properties_ShouldBeSettable()
    {
        // Arrange
        var message = new OutboxMessage();

        // Act
        message.EventType = "DeclarationAcceptedIntegrationEvent";
        message.AggregateType = "CustomsDeclaration";
        message.AggregateId = Guid.NewGuid();
        message.PayloadJson = "{\"declarationId\":\"8b7d2e5f-0000-0000-0000-000000000001\"}";
        message.Status = "Published";
        message.PublishedAtUtc = DateTime.UtcNow;

        // Assert
        Assert.Equal("DeclarationAcceptedIntegrationEvent", message.EventType);
        Assert.Equal("CustomsDeclaration", message.AggregateType);
        Assert.NotEqual(Guid.Empty, message.AggregateId);
        Assert.Contains("declarationId", message.PayloadJson);
        Assert.Equal("Published", message.Status);
        Assert.NotNull(message.PublishedAtUtc);
    }
}
