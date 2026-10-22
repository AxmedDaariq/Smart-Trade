using System;
using System.Threading;
using System.Threading.Tasks;
using Integration.Application.Commands;
using Integration.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Integration.Tests.Application.Commands;

public class ProcessWebhookCommandHandlerTests
{
    private const string PartnerSecret = "test-secret-value-for-unit-testing-only";
    private const string UnknownPartner = "UNKNOWN-PARTNER";

    private readonly HmacExternalRequestSigner _signer = new();
    private readonly TestSecretStore _secrets = new();
    private readonly ProcessWebhookCommandHandler _handler;

    public ProcessWebhookCommandHandlerTests()
    {
        _secrets.Add("PTR-001", PartnerSecret);
        _secrets.Add("ACME", PartnerSecret);
        _handler = new ProcessWebhookCommandHandler(_signer, _secrets, NullLogger<ProcessWebhookCommandHandler>.Instance);
    }

    private ProcessWebhookCommand BuildCommand(
        string payload = "{\"event\":\"vessel.arrived\",\"reference\":\"VES-1\"}",
        string partnerCode = "PTR-001",
        Guid? tenantId = null,
        DateTime? timestamp = null,
        string signature = null)
    {
        return new ProcessWebhookCommand
        {
            TenantId = tenantId ?? Guid.NewGuid(),
            PartnerCode = partnerCode,
            Payload = payload,
            Signature = signature ?? _signer.GenerateSignature(payload, PartnerSecret),
            Timestamp = timestamp ?? DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Handle_WithValidSignatureAndFreshTimestamp_ShouldReturnTrue()
    {
        // Arrange
        var command = BuildCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: a correctly signed webhook is accepted for asynchronous processing (202 semantics)
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WithoutTimestampHeader_ShouldReturnFalse()
    {
        // Arrange: the X-Timestamp header is mandatory so a captured payload cannot be replayed
        var command = BuildCommand(timestamp: null);
        command.Timestamp = null;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WithTimestampOutsideReplayWindow_ShouldReturnFalse()
    {
        // Arrange: payload captured more than the maximum skew in the past
        var command = BuildCommand(timestamp: DateTime.UtcNow - TimeSpan.FromMinutes(10));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: replayed webhooks are rejected
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WithEmptyPayload_ShouldReturnTrueWhenCorrectlySigned()
    {
        // Arrange: an empty body is unusual but a correctly signed one is still authentic
        var command = BuildCommand(payload: string.Empty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sha256=not-the-real-signature")]
    [InlineData("invalid-format")]
    public async Task Handle_WithInvalidSignature_ShouldRejectPayload(string signature)
    {
        // A08: HMAC verification is wired up - bad signatures are rejected instead of accepted.
        var command = BuildCommand(signature: signature);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenPayloadWasTamperedWith_ShouldReturnFalse()
    {
        // Arrange: signature computed over a different body
        var command = BuildCommand();
        command.Payload = "{\"event\":\"vessel.arrived\",\"reference\":\"VES-999\"}";

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenPartnerHasNoConfiguredSecret_ShouldReturnFalse()
    {
        // Arrange: fail closed for unknown partners
        var command = BuildCommand(partnerCode: UnknownPartner);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenCalledRepeatedly_ShouldRemainStable()
    {
        // Arrange: webhooks are retried by the partner within a short window
        var command = BuildCommand();

        // Act
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

    private sealed class TestSecretStore : IWebhookSecretStore
    {
        private readonly System.Collections.Generic.Dictionary<string, string> _secrets = new(StringComparer.OrdinalIgnoreCase);

        public void Add(string partnerCode, string secret) => _secrets[partnerCode] = secret;

        public string? GetSecret(string? partnerCode) =>
            partnerCode is not null && _secrets.TryGetValue(partnerCode, out var secret) ? secret : null;
    }
}
