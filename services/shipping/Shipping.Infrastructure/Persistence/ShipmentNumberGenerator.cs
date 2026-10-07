using Shipping.Application.Commands;

namespace Shipping.Infrastructure.Persistence;

/// <summary>
/// Generates unique, tenant-scoped shipment reference numbers.
/// Format: SHP-{TenantPrefix}-{YYYYMMDD}-{Sequence}
/// e.g.  SHP-A1B2-20241015-000042
/// </summary>
public class ShipmentNumberGenerator : IShipmentNumberGenerator
{
    private static int _sequence = 0;

    public string Generate(Guid tenantId)
    {
        var tenantPrefix = tenantId.ToString("N")[..4].ToUpperInvariant();
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var seq = Interlocked.Increment(ref _sequence);
        return $"SHP-{tenantPrefix}-{datePart}-{seq:D6}";
    }
}
