using System;
using System.Security.Cryptography;
using System.Text;

namespace Integration.Application.Services;

/// <summary>
/// A08: HMAC-SHA256 webhook authentication using constant-time comparison, so partner payloads
/// cannot be forged and signature checks cannot be brute forced through timing differences.
/// </summary>
public sealed class HmacExternalRequestSigner : IExternalRequestSigner
{
    private const string HmacSha256 = "HMAC-SHA256";

    public string GenerateSignature(string payload, string secret, string algorithm = HmacSha256)
    {
        EnsureSupportedAlgorithm(algorithm);
        if (string.IsNullOrEmpty(secret)) throw new ArgumentException("A webhook secret is required.", nameof(secret));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload ?? string.Empty));
        return "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool VerifySignature(string payload, string secret, string signature, string algorithm = HmacSha256)
    {
        EnsureSupportedAlgorithm(algorithm);

        // Fail closed: a missing secret or signature is never a valid webhook.
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(signature)) return false;

        var expected = GenerateSignature(payload, secret, algorithm);
        var actual = signature.Trim().ToLowerInvariant();

        if (expected.Length != actual.Length) return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(actual));
    }

    private static void EnsureSupportedAlgorithm(string algorithm)
    {
        if (!string.Equals(algorithm, HmacSha256, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Webhook signature algorithm '{algorithm}' is not supported.");
    }
}
