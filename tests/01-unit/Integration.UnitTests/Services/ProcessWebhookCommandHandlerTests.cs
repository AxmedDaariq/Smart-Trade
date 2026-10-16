using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Integration.Application.Commands;
using Integration.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Integration.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="ProcessWebhookCommandHandler"/>.
/// Validates HMAC signature verification, timestamp freshness, secret store lookups,
/// and the fail-closed security posture of the webhook processing pipeline.
/// </summary>
public sealed class ProcessWebhookCommandHandlerTests
{
    private readonly Mock<IExternalRequestSigner> _signerMock = new();
    private readonly Mock<IWebhookSecretStore> _secretStoreMock = new();
    private readonly Mock<ILogger<ProcessWebhookCommandHandler>> _loggerMock = new();
    private readonly ProcessWebhookCommandHandler _handler;

    public ProcessWebhookCommandHandlerTests()
    {
        _handler = new ProcessWebhookCommandHandler(
            _signerMock.Object,
            _secretStoreMock.Object,
            _loggerMock.Object);
    }

    // ─── Happy path ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidSignatureAndTimestamp_ReturnsTrue()
    {
        // Arrange
        var partnerCode = "maersk";
        var secret = "super-secret-key";
        var payload = "{\"event\":\"container.unloaded\"}";
        var signature = "sha256=abc123";

        _secretStoreMock.Setup(s => s.GetSecret(partnerCode)).Returns(secret);
        _signerMock.Setup(s => s.VerifySignature(payload, secret, signature, It.IsAny<string>())).Returns(true);

        var command = new ProcessWebhookCommand
        {
            PartnerCode = partnerCode,
            Payload = payload,
            Signature = signature,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    // ─── Missing timestamp ───────────────────────────────────────────────────

    [Fact]
    public async Task Handle_NullTimestamp_ReturnsFalse()
    {
        // Arrange
        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = null
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("a missing timestamp must cause rejection");
    }

    // ─── Stale timestamp (replay protection) ─────────────────────────────────

    [Fact]
    public async Task Handle_StaleTimestamp_ReturnsFalse()
    {
        // Arrange – timestamp 10 minutes in the past, window is 5 minutes
        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = DateTime.UtcNow.AddMinutes(-10)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("a stale timestamp outside the replay window must cause rejection");
    }

    [Fact]
    public async Task Handle_FutureTimestamp_ReturnsFalse()
    {
        // Arrange – timestamp 10 minutes in the future
        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = DateTime.UtcNow.AddMinutes(10)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("a future timestamp outside the skew window must cause rejection");
    }

    // ─── Unknown partner (no secret) ─────────────────────────────────────────

    [Fact]
    public async Task Handle_UnknownPartner_ReturnsFalse()
    {
        // Arrange
        _secretStoreMock.Setup(s => s.GetSecret("unknown")).Returns((string?)null);

        var command = new ProcessWebhookCommand
        {
            PartnerCode = "unknown",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("an unknown partner without a configured secret must be rejected");
    }

    [Fact]
    public async Task Handle_EmptySecret_ReturnsFalse()
    {
        // Arrange
        _secretStoreMock.Setup(s => s.GetSecret("partner")).Returns("");

        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("an empty secret must fail closed");
    }

    // ─── Bad signature ───────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_InvalidSignature_ReturnsFalse()
    {
        // Arrange
        var secret = "valid-secret";
        _secretStoreMock.Setup(s => s.GetSecret("partner")).Returns(secret);
        _signerMock.Setup(s => s.VerifySignature(It.IsAny<string>(), secret, "sha256=wrong", It.IsAny<string>())).Returns(false);

        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=wrong",
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse("a failed HMAC signature check must reject the webhook");
    }

    // ─── Verify invocation order ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_ChecksTimestampBeforeSecretAndSignature()
    {
        // Arrange – null timestamp should reject before ever calling the secret store
        var command = new ProcessWebhookCommand
        {
            PartnerCode = "partner",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = null
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _secretStoreMock.Verify(s => s.GetSecret(It.IsAny<string>()), Times.Never,
            "the secret store should not be consulted when the timestamp is missing");
        _signerMock.Verify(s => s.VerifySignature(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never,
            "the signer should not be consulted when the timestamp is missing");
    }

    [Fact]
    public async Task Handle_ChecksSecretBeforeSignature()
    {
        // Arrange – valid timestamp but no secret
        _secretStoreMock.Setup(s => s.GetSecret("no-secret")).Returns((string?)null);

        var command = new ProcessWebhookCommand
        {
            PartnerCode = "no-secret",
            Payload = "{}",
            Signature = "sha256=abc",
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _signerMock.Verify(s => s.VerifySignature(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never,
            "the signer should not be consulted when there is no secret for the partner");
    }
}

/// <summary>
/// Unit tests for <see cref="HmacExternalRequestSigner"/>.
/// Validates correct HMAC-SHA256 generation and constant-time verification.
/// </summary>
public sealed class HmacExternalRequestSignerTests
{
    private readonly HmacExternalRequestSigner _signer = new();

    [Fact]
    public void GenerateSignature_ReturnsHexPrefixedHash()
    {
        // Arrange
        var payload = "{\"event\":\"test\"}";
        var secret = "my-secret";

        // Act
        var signature = _signer.GenerateSignature(payload, secret);

        // Assert
        signature.Should().StartWith("sha256=");
        signature.Length.Should().BeGreaterThan("sha256=".Length);
    }

    [Fact]
    public void GenerateSignature_SameInputProducesSameOutput()
    {
        var s1 = _signer.GenerateSignature("payload", "secret");
        var s2 = _signer.GenerateSignature("payload", "secret");

        s1.Should().Be(s2);
    }

    [Fact]
    public void GenerateSignature_DifferentPayloadsProduceDifferentSignatures()
    {
        var s1 = _signer.GenerateSignature("payload-a", "secret");
        var s2 = _signer.GenerateSignature("payload-b", "secret");

        s1.Should().NotBe(s2);
    }

    [Fact]
    public void GenerateSignature_DifferentSecretsProduceDifferentSignatures()
    {
        var s1 = _signer.GenerateSignature("payload", "secret-a");
        var s2 = _signer.GenerateSignature("payload", "secret-b");

        s1.Should().NotBe(s2);
    }

    [Fact]
    public void GenerateSignature_EmptySecret_ThrowsArgumentException()
    {
        var act = () => _signer.GenerateSignature("payload", "");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GenerateSignature_NullPayload_DoesNotThrow()
    {
        // The implementation uses payload ?? string.Empty
        var act = () => _signer.GenerateSignature(null!, "secret");
        act.Should().NotThrow();
    }

    [Fact]
    public void GenerateSignature_UnsupportedAlgorithm_ThrowsNotSupportedException()
    {
        var act = () => _signer.GenerateSignature("payload", "secret", "HMAC-SHA512");
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void VerifySignature_ValidSignature_ReturnsTrue()
    {
        var payload = "test-payload";
        var secret = "test-secret";
        var signature = _signer.GenerateSignature(payload, secret);

        _signer.VerifySignature(payload, secret, signature).Should().BeTrue();
    }

    [Fact]
    public void VerifySignature_InvalidSignature_ReturnsFalse()
    {
        _signer.VerifySignature("payload", "secret", "sha256=wrong").Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_EmptySignature_ReturnsFalse()
    {
        _signer.VerifySignature("payload", "secret", "").Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_EmptySecret_ReturnsFalse()
    {
        _signer.VerifySignature("payload", "", "sha256=abc").Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_NullSecret_ReturnsFalse()
    {
        _signer.VerifySignature("payload", null!, "sha256=abc").Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_NullSignature_ReturnsFalse()
    {
        _signer.VerifySignature("payload", "secret", null!).Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_CaseInsensitive()
    {
        var payload = "hello";
        var secret = "key";
        var signature = _signer.GenerateSignature(payload, secret);

        // The implementation normalises to lowercase, verify case-insensitivity
        _signer.VerifySignature(payload, secret, signature.ToUpperInvariant()).Should().BeTrue();
    }
}
