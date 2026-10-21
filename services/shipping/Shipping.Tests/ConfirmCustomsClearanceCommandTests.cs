using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Entities;
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

        var shipment = new Shipment(
            tenantId: tenantId,
            shipmentNumber: "TEST-SHIP-001",
            shipperName: "Test Shipper",
            shipperReference: "REF-001",
            consigneeName: "Test Consignee",
            consigneeReference: "CON-001",
            originCountryCode: "SO",
            originPortCode: "SOBER",
            originPortName: "Berbera",
            destinationCountryCode: "AE",
            destinationPortCode: "AEJEA",
            destinationPortName: "Jebel Ali",
            transportMode: TransportMode.Sea,
            cargoType: CargoType.FCL,
            incoterm: Incoterm.CIF,
            currency: "USD",
            createdBy: userId);

        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        return (ctx, shipment.Id, tenantId, userId);
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
