using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class ShipmentNumberGeneratorTests
{
    [Fact]
    public void Generate_ReturnsExpectedFormat()
    {
        var generator = new ShipmentNumberGenerator();
        var tenantId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

        var number = generator.Generate(tenantId);

        // Format: SHP-{TenantPrefix}-{yyyyMMdd}-{Sequence}
        Assert.StartsWith("SHP-AAAA-", number);
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        Assert.Contains($"-{datePart}-", number);
    }

    [Fact]
    public void Generate_UsesFirstFourHexCharsOfTenantIdAsPrefix()
    {
        var generator = new ShipmentNumberGenerator();
        var tenantId = Guid.Parse("12345678-0000-0000-0000-000000000001");

        var number = generator.Generate(tenantId);

        Assert.StartsWith("SHP-1234-", number);
    }

    [Fact]
    public void Generate_IsUniqueAcrossCalls()
    {
        var generator = new ShipmentNumberGenerator();
        var tenantId = Guid.NewGuid();

        var first = generator.Generate(tenantId);
        var second = generator.Generate(tenantId);
        var third = generator.Generate(tenantId);

        Assert.NotEqual(first, second);
        Assert.NotEqual(second, third);
        Assert.NotEqual(first, third);
    }

    [Fact]
    public void Generate_EmbedsIncrementalSequence()
    {
        var generator = new ShipmentNumberGenerator();
        var tenantId = Guid.NewGuid();

        var first = generator.Generate(tenantId);
        var second = generator.Generate(tenantId);

        var firstSeq = long.Parse(first.Split('-').Last());
        var secondSeq = long.Parse(second.Split('-').Last());

        Assert.Equal(firstSeq + 1, secondSeq);
    }
}
