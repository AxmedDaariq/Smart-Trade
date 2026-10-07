using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class ConfirmCustomsClearanceCommandTests
{
    private static async Task<(ShippingDbContext ctx, Guid shipmentId, Guid tenantId, Guid userId)> SeedShipmentAsync()
    {
        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new ShippingDbContext(options);

        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var createHandler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());
        var shipmentId = await createHandler.Handle(new CreateShipmentCommand
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
            GrossWeightKg = 10000,
            PackageCount = 200,
            UserId = userId
        }, CancellationToken.None);

        return (ctx, shipmentId, tenantId, userId);
    }

    [Fact]
    public async Task Handle_ValidCommand_ClearsCustomsAndTransitionsStatus()
    {
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);
        var customsCaseId = Guid.NewGuid();

        await handler.Handle(new ConfirmCustomsClearanceCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            CustomsCaseId = customsCaseId,
            CustomsCaseNumber = "CUS-2024-0001",
            UserId = userId
        }, CancellationToken.None);

        var shipment = await ctx.Shipments
            .Include(s => s.StatusHistories)
            .Include(s => s.TrackingEvents)
            .FirstAsync(s => s.Id == shipmentId);

        Assert.True(shipment.IsCustomsCleared);
        Assert.Equal(customsCaseId, shipment.CustomsCaseId);
        Assert.Equal("CUS-2024-0001", shipment.CustomsCaseNumber);
        Assert.NotNull(shipment.CustomsClearedAtUtc);
        Assert.Equal(ShipmentStatus.CustomsCleared, shipment.Status);

        var history = Assert.Single(shipment.StatusHistories);
        Assert.Equal("Draft", history.FromStatus);
        Assert.Equal("CustomsCleared", history.ToStatus);

        var trackingEvent = Assert.Single(shipment.TrackingEvents);
        Assert.Equal(TrackingEventType.CustomsCleared, trackingEvent.EventType);
    }

    [Fact]
    public async Task Handle_AlreadyCleared_ThrowsInvalidOperation()
    {
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);

        await handler.Handle(new ConfirmCustomsClearanceCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            CustomsCaseId = Guid.NewGuid(),
            CustomsCaseNumber = "CUS-2024-0001",
            UserId = userId
        }, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ConfirmCustomsClearanceCommand
            {
                TenantId = tenantId,
                ShipmentId = shipmentId,
                CustomsCaseId = Guid.NewGuid(),
                CustomsCaseNumber = "CUS-2024-0002",
                UserId = userId
            }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownShipment_ThrowsInvalidOperation()
    {
        var (ctx, _, tenantId, userId) = await SeedShipmentAsync();
        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ConfirmCustomsClearanceCommand
            {
                TenantId = tenantId,
                ShipmentId = Guid.NewGuid(),
                CustomsCaseId = Guid.NewGuid(),
                CustomsCaseNumber = "CUS-2024-0001",
                UserId = userId
            }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongTenant_ThrowsInvalidOperation()
    {
        var (ctx, shipmentId, _, userId) = await SeedShipmentAsync();
        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ConfirmCustomsClearanceCommand
            {
                TenantId = Guid.NewGuid(),
                ShipmentId = shipmentId,
                CustomsCaseId = Guid.NewGuid(),
                CustomsCaseNumber = "CUS-2024-0001",
                UserId = userId
            }, CancellationToken.None));
    }
}
