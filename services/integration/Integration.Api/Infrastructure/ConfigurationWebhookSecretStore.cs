using System;
using System.Text.RegularExpressions;
using Integration.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Integration.Api.Infrastructure;

/// <summary>
/// Reads per-partner webhook secrets from configuration
/// (<c>Webhooks:Partners:{partnerCode}:Secret</c>, normally injected from a secret manager).
/// Unknown partners return <c>null</c> so the handler rejects the payload (fail closed).
/// </summary>
public sealed class ConfigurationWebhookSecretStore : IWebhookSecretStore
{
    // A05/A03: the partner code becomes part of a configuration index path, so only a strict
    // token is accepted. This blocks path traversal such as "../../Jwt/Secret".
    private static readonly Regex PartnerCodePattern = new("^[A-Za-z0-9_-]{1,32}$", RegexOptions.Compiled);

    private readonly IConfiguration _configuration;

    public ConfigurationWebhookSecretStore(IConfiguration configuration) => _configuration = configuration;

    public string? GetSecret(string partnerCode)
    {
        if (string.IsNullOrWhiteSpace(partnerCode) || !PartnerCodePattern.IsMatch(partnerCode))
            return null;

        return _configuration[$"Webhooks:Partners:{partnerCode}:Secret"];
    }
}
