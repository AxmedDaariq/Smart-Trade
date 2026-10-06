using System.Net;
using System.Net.Sockets;

namespace BuildingBlocks.Web;

/// <summary>
/// A10 (SSRF) guard for any URL that originates from a partner, tenant or end user - for example
/// the outbound delivery address of a <c>WebhookSubscription.TargetUrl</c> or a partner-provided
/// callback.
///
/// Operator supplied configuration (<c>ConnectionStrings</c>, internal service base URLs) is
/// trusted and must not go through this guard, otherwise in-cluster HTTP calls would be blocked.
///
/// The check fails closed: an unparsable URL, a non-HTTPS scheme, a URL that carries credentials,
/// a host resolving to loopback/private/link-local/metadata space, or an unresolvable host are all
/// treated as unsafe. DNS is resolved at validation time; callers that are rate sensitive should
/// still pin the resolved address for the actual request to fully eliminate DNS rebinding.
/// </summary>
public static class PlatformUrlGuard
{
    /// <summary>Returns <c>true</c> when <paramref name="candidate"/> is safe to request from the server.</summary>
    public static bool IsSafe(string candidate) => TryValidate(candidate, out _);

    /// <summary>Returns <c>true</c> when <paramref name="candidate"/> is unsafe, with the reason.</summary>
    public static bool TryGetRejectionReason(string candidate, out string reason) => !TryValidate(candidate, out reason);

    /// <summary>Throws when <paramref name="candidate"/> is not a safe outbound URL.</summary>
    public static Uri EnsureSafe(string candidate, string parameterName = "url")
    {
        if (!TryValidate(candidate, out var reason))
            throw new InvalidOperationException($"{parameterName} was rejected by the outbound URL guard: {reason}");

        return new Uri(candidate, UriKind.Absolute);
    }

    private static bool TryValidate(string candidate, out string reason)
    {
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(candidate))
        {
            reason = "value is empty";
            return false;
        }

        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
        {
            reason = "value is not an absolute URI";
            return false;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            reason = "only https URLs are allowed";
            return false;
        }

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            reason = "embedded credentials are not allowed";
            return false;
        }

        if (uri.Port <= 0)
        {
            reason = "invalid port";
            return false;
        }

        IPAddress[] addresses;
        try
        {
            addresses = Dns.GetHostAddresses(uri.DnsSafeHost);
        }
        catch (Exception ex) when (ex is SocketException or ArgumentException or InvalidOperationException)
        {
            reason = "host could not be resolved";
            return false;
        }

        if (addresses.Length == 0)
        {
            reason = "host did not resolve to any address";
            return false;
        }

        foreach (var address in addresses)
        {
            if (IsBlockedAddress(address))
            {
                reason = $"host resolves to a blocked address range ({address})";
                return false;
            }
        }

        return true;
    }

    private static bool IsBlockedAddress(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6) address = address.MapToIPv4();

        if (IPAddress.IsLoopback(address)) return true;

        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var octets = address.GetAddressBytes();
            return octets[0] switch
            {
                0 => true,                                  // 0.0.0.0/8
                10 => true,                                 // RFC1918
                100 when (octets[1] & 0xC0) == 64 => true,  // CGNAT 100.64/10
                127 => true,                                // loopback
                169 when octets[1] == 254 => true,          // link-local / cloud metadata
                172 when octets[1] >= 16 && octets[1] <= 31 => true, // RFC1918
                192 when octets[1] == 168 => true,          // RFC1918
                >= 224 => true,                             // multicast, reserved, broadcast
                _ => false
            };
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (address.IsIPv6LinkLocal || address.IsIPv6SiteLocal ||
                address.IsIPv6Multicast || address.IsIPv6UniqueLocal)
                return true;

            return address.Equals(IPAddress.IPv6Loopback) || address.Equals(IPAddress.IPv6Any);
        }

        // Unknown address family: fail closed.
        return true;
    }
}
