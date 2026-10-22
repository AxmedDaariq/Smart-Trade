using System;
using Customs.Application.Commands;

namespace Customs.Infrastructure.Services;

/// <summary>
/// Default implementation of <see cref="ICustomsCaseNumberGenerator"/>
/// producing formatted case tracking numbers like "CC-YYYYMMDD-XXXXXXXX".
/// </summary>
public class CustomsCaseNumberGenerator : ICustomsCaseNumberGenerator
{
    public string Generate(Guid tenantId)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"CC-{date}-{suffix}";
    }
}
