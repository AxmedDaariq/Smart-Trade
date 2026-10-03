using System;
using System.Threading;
using System.Threading.Tasks;
using Integration.Application.Commands;

namespace Integration.Tests.Application.Commands;

public class ProcessWebhookCommandHandlerTests
{
    private readonly ProcessWebhookCommandHandler _handler = new();

    private static ProcessWebhookCommand BuildCommand(
        string payload = "{\"event\":\"vessel.arrived\",\"reference\":\"VES-1\"}",
        string signature = "sha256=3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
        DateTime? timestamp = null,
        string partnerCode = "PTR-001",
        Guid? tenantId = null)
    {
        return new ProcessWebhookCommand
        {
            TenantId = tenantId ?? Guid.NewGuid(),
            PartnerCode = partnerCode,
            Payload = payload,
            Signature = signature,
            Timestamp = timestamp
        };
    }

    [Fact]
    public async Task Handle_WithWellFormedWebhook_ShouldReturnTrue()
    {
        // Arrange
        var command = BuildCommand(timestamp: DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: a valid webhook is accepted for asynchronous processing (202 semantics)
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WithoutTimestampHeader_ShouldReturnTrue()
    {
        // Arrange: partners are not required to send the X-Timestamp header
        var command = BuildCommand(timestamp: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WithEmptyPayload_ShouldReturnTrueWithoutThrowing()
    {
        // Arrange
        var command = BuildCommand(payload: string.Empty, signature: string.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sha256=not-the-real-signature")]
    [InlineData("invalid-format")]
    public async Task Handle_WhenSignatureVerificationIsNotWiredUpYet_ShouldAcceptPayload(string signature)
    {
        // NOTE: signature verification (IExternalRequestSigner) is not implemented yet,
        // so the handler currently accepts every payload. When HMAC verification is wired
        // in, flip this test to assert that bad signatures are rejected.
        var command = BuildCommand(signature: signature);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (characterizes today's stub behaviour)
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenCalledRepeatedly_ShouldRemainStable()
    {
        // Arrange
        var command = BuildCommand();

        // Act: webhooks can be retried by the partner within a short window
        for (var i = 0; i < 3; i++)
        {
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public async Task Handle_ShouldNotMutateTheCommand()
    {
        // Arrange
        var command = BuildCommand();
        var originalPayload = command.Payload;
        var originalSignature = command.Signature;
        var originalTenantId = command.TenantId;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert: handlers must not rewrite inbound webhook data
        Assert.Equal(originalPayload, command.Payload);
        Assert.Equal(originalSignature, command.Signature);
        Assert.Equal(originalTenantId, command.TenantId);
    }

    [Fact]
    public void ProcessWebhookCommand_ShouldBeAReadableTransportObject()
    {
        // Arrange & Act
        var command = BuildCommand(partnerCode: "ACME", payload: "{}");

        // Assert
        Assert.Equal("ACME", command.PartnerCode);
        Assert.Equal("{}", command.Payload);
        Assert.NotEqual(Guid.Empty, command.TenantId);
    }
}
