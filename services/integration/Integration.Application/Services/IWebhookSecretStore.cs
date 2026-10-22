namespace Integration.Application.Services;

/// <summary>
/// Resolves the shared secret used to authenticate an inbound webhook for a partner.
/// Implementations must fail closed: an unknown or unconfigured partner returns <c>null</c>,
/// which makes <see cref="Commands.ProcessWebhookCommandHandler"/> reject the payload.
/// </summary>
public interface IWebhookSecretStore
{
    /// <summary>Returns the shared HMAC secret for <paramref name="partnerCode"/>, or <c>null</c>.</summary>
    string? GetSecret(string partnerCode);
}
