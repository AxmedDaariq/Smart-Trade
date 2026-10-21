using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class UpdateShipmentStatusCommandTests
{
    private static async Task<(ShippingDbContext ctx, Guid shipmentId)> SeedShipmentAsync()
    {
        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new ShippingDbContext(options);

        var shipment = new Shipment(
            tenantId: Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
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
            incoterm: Incoterm.FOB,
            currency: "USD",
            createdBy: Guid.NewGuid());

        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        return (ctx, shipment.Id);
    }

    [Fact]
    public async Task Handle_ValidTransition_UpdatesStatusAndRecordsHistory()
    {
        var (ctx, shipmentId) = await SeedShipmentAsync();
        var handler = new UpdateShipmentStatusCommandHandler(ctx);

        await handler.Handle(new UpdateShipmentStatusCommand
        {
            TenantId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            ShipmentId = shipmentId,
            NewStatus = ShipmentStatus.Booked,
            Reason = "Booking confirmed by carrier",
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        var shipment = await ctx.Shipments
            .Include(s => s.StatusHistories)
            .FirstAsync(s => s.Id == shipmentId);

        Assert.Equal(ShipmentStatus.Booked, shipment.Status);
        Assert.Single(shipment.StatusHistories);
        Assert.Equal("Draft", shipment.StatusHistories.First().FromStatus);
        Assert.Equal("Booked", shipment.StatusHistories.First().ToStatus);
    }

    [Fact]
    public async Task Handle_TransitionFromDelivered_ThrowsInvalidOperation()
    {
        var (ctx, shipmentId) = await SeedShipmentAsync();
        var handler = new UpdateShipmentStatusCommandHandler(ctx);

        // First move to Delivered
        await handler.Handle(new UpdateShipmentStatusCommand
        {
            TenantId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
            ShipmentId = shipmentId,
            NewStatus = ShipmentStatus.Delivered,
            UserId = Guid.NewGuid()
        }, CancellationToken.None);

        // Then try to move again — should fail
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new UpdateShipmentStatusCommand
            {
                TenantId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                ShipmentId = shipmentId,
                NewStatus = ShipmentStatus.InTransit,
                UserId = Guid.NewGuid()
            }, CancellationToken.None));
    }
}
