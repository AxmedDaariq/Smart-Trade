using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for the inbox de-duplication <see cref="InboxMessage"/>.
/// </summary>
public class InboxMessageTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var message = new InboxMessage();
        var after   = DateTime.UtcNow.AddSeconds(1);

        message.Id.Should().NotBe(Guid.Empty);
        message.MessageId.Should().Be(Guid.Empty);
        message.MessageType.Should().BeEmpty();
        message.Consumer.Should().BeEmpty();
        message.TenantId.Should().BeNull();
        message.PayloadHash.Should().BeEmpty();
        message.ProcessedAtUtc.Should().BeNull();
        message.Status.Should().Be("Pending");
        message.RetryCount.Should().Be(0);
        message.LastError.Should().BeNull();
        message.ReceivedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var messageId = Guid.NewGuid();
        var processed = DateTime.UtcNow;

        var message = new InboxMessage
        {
            MessageId     = messageId,
            MessageType   = "DeclarationCreated",
            Consumer      = "Audit.Ingestion",
            TenantId      = Guid.NewGuid(),
            PayloadHash   = "sha256-xyz",
            ProcessedAtUtc= processed,
            Status        = "Processed"
        };

        message.MessageId.Should().Be(messageId);
        message.MessageType.Should().Be("DeclarationCreated");
        message.Consumer.Should().Be("Audit.Ingestion");
        message.PayloadHash.Should().Be("sha256-xyz");
        message.ProcessedAtUtc.Should().Be(processed);
        message.Status.Should().Be("Processed");
    }
}
