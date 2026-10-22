using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class CreateShipmentCommandTests
{
    private ShippingDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ShippingDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesShipmentAndReturnsId()
    {
        // Arrange
        await using var ctx = CreateInMemoryContext();
        var handler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());

        var command = new CreateShipmentCommand
        {
            TenantId = Guid.NewGuid(),
            ShipperName = "Acme Exports Ltd",
            ShipperReference = "ACME-2024-001",
            ConsigneeName = "Global Imports Co",
            ConsigneeReference = "GI-2024-555",
            OriginCountryCode = "SO",
            OriginPortCode = "SOBER",
            OriginPortName = "Berbera",
            DestinationCountryCode = "AE",
            DestinationPortCode = "AEJEA",
            DestinationPortName = "Jebel Ali",
            TransportMode = TransportMode.Sea,
            CargoType = CargoType.FCL,
            Incoterm = Incoterm.FOB,
            Currency = "USD",
            GrossWeightKg = 24000,
            NetWeightKg = 22500,
            VolumeM3 = 28.5m,
            PackageCount = 480,
            DeclaredValue = 125000,
            UserId = Guid.NewGuid(),
            Lines = new()
            {
                new()
                {
                    LineNumber = 1,
                    Description = "Sesame seeds, hulled",
                    HsCode = "1207.40",
                    CountryOfOrigin = "SO",
                    Quantity = 480,
                    UnitOfMeasure = "BAG",
                    GrossWeightKg = 24000,
                    NetWeightKg = 22500,
                    UnitValue = 260.42m,
                    TotalValue = 125000,
                    Currency = "USD"
                }
            }
        };

        // Act
        var shipmentId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, shipmentId);

        var saved = await ctx.Shipments
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        Assert.NotNull(saved);
        Assert.Equal("Acme Exports Ltd", saved.ShipperName);
        Assert.Equal(TransportMode.Sea, saved.TransportMode);
        Assert.Equal(Domain.Enums.ShipmentStatus.Draft, saved.Status);
        Assert.Single(saved.Lines);
        Assert.Equal("1207.40", saved.Lines.First().HsCode);
        Assert.StartsWith("SHP-", saved.ShipmentNumber);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShipmentNumberIsUnique()
    {
        // Arrange
        await using var ctx = CreateInMemoryContext();
        var generator = new ShipmentNumberGenerator();
        var handler = new CreateShipmentCommandHandler(ctx, generator);
        var tenantId = Guid.NewGuid();

        var command = new CreateShipmentCommand
        {
            TenantId = tenantId,
            ShipperName = "Test Shipper",
            ShipperReference = "REF-001",
            ConsigneeName = "Test Consignee",
            ConsigneeReference = "CON-001",
            OriginCountryCode = "SO",
            OriginPortCode = "SOBER",
            OriginPortName = "Berbera",
            DestinationCountryCode = "AE",
            DestinationPortCode = "AEJEA",
            DestinationPortName = "Jebel Ali",
            TransportMode = TransportMode.Sea,
            CargoType = CargoType.FCL,
            Incoterm = Incoterm.CIF,
            Currency = "USD",
            GrossWeightKg = 5000,
            PackageCount = 100,
            UserId = Guid.NewGuid()
        };

        // Act
        var id1 = await handler.Handle(command, CancellationToken.None);
        command.ShipperReference = "REF-002"; // change to avoid unique constraint
        var id2 = await handler.Handle(command, CancellationToken.None);

        // Assert
        var s1 = await ctx.Shipments.FindAsync(id1);
        var s2 = await ctx.Shipments.FindAsync(id2);

        Assert.NotEqual(s1!.ShipmentNumber, s2!.ShipmentNumber);
    }
}
