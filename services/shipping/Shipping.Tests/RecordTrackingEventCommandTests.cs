using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class RecordTrackingEventCommandTests
{
    private static async Task<(DbContextOptions<ShippingDbContext> options, Guid shipmentId, Guid tenantId, Guid userId)> SeedShipmentAsync()
    {
        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var ctx = new ShippingDbContext(options);

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
            incoterm: Incoterm.FOB,
            currency: "USD",
            createdBy: userId);

        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        return (options, shipment.Id, tenantId, userId);
    }

    [Fact]
    public async Task Handle_ValidEvent_AddsTrackingEventAndReturnsId()
    {
        var (options, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        var eventId = await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.DocumentsIssued,
            Location = "SOBER",
            Description = "Bill of Lading issued",
            UserId = userId
        }, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, eventId);

        var shipment = await ctx.Shipments
            .Include(s => s.TrackingEvents)
            .FirstAsync(s => s.Id == shipmentId);

        var evt = Assert.Single(shipment.TrackingEvents);
        Assert.Equal(eventId, evt.Id);
        Assert.Equal(TrackingEventType.DocumentsIssued, evt.EventType);
        Assert.Equal("SOBER", evt.Location);
        Assert.Equal("Bill of Lading issued", evt.Description);
        Assert.Equal(userId, evt.RecordedBy);
    }

    [Fact]
    public async Task Handle_PickupEvent_AutoTransitionsStatusToPickedUp()
    {
        var (options, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.CargoPickedUp,
            Location = "Berbera ICD",
            UserId = userId
        }, CancellationToken.None);

        var shipment = await ctx.Shipments
            .Include(s => s.StatusHistories)
            .Include(s => s.TrackingEvents)
            .FirstAsync(s => s.Id == shipmentId);

        Assert.Equal(ShipmentStatus.PickedUp, shipment.Status);

        var history = Assert.Single(shipment.StatusHistories);
        Assert.Equal("Draft", history.FromStatus);
        Assert.Equal("PickedUp", history.ToStatus);
    }

    [Fact]
    public async Task Handle_DeliveredEvent_AutoTransitionsStatusToDelivered()
    {
        var (options, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.Delivered,
            Location = "Jebel Ali Terminal",
            Description = "Delivered to consignee",
            UserId = userId
        }, CancellationToken.None);

        var shipment = await ctx.Shipments.FirstAsync(s => s.Id == shipmentId);

        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
    }

    [Fact]
    public async Task Handle_EventWithoutAutoTransition_KeepsCurrentStatus()
    {
        var (options, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.Transshipped,
            Location = "Port Sudan",
            UserId = userId
        }, CancellationToken.None);

        var shipment = await ctx.Shipments.FirstAsync(s => s.Id == shipmentId);

        // Transshipped has no auto-transition rule — status stays Draft
        Assert.Equal(ShipmentStatus.Draft, shipment.Status);
    }

    [Fact]
    public async Task Handle_EnrichesEventWithGpsAndCarrierMetadata()
    {
        var (options, shipmentId, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.DepartedOrigin,
            Location = "Gulf of Aden",
            Latitude = 12.7833,
            Longitude = 45.0,
            VehicleIdentifier = "VSL-9988",
            CarrierEventCode = "DEP-001",
            UserId = userId
        }, CancellationToken.None);

        var evt = await ctx.TrackingEvents.FirstAsync(t => t.ShipmentId == shipmentId);

        Assert.Equal(12.7833, evt.Latitude);
        Assert.Equal(45.0, evt.Longitude);
        Assert.Equal("VSL-9988", evt.VehicleIdentifier);
        Assert.Equal("DEP-001", evt.CarrierEventCode);
    }

    [Fact]
    public async Task Handle_UnknownShipment_ThrowsInvalidOperation()
    {
        var (options, _, tenantId, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new RecordTrackingEventCommand
            {
                TenantId = tenantId,
                ShipmentId = Guid.NewGuid(),
                EventType = TrackingEventType.OrderCreated,
                Location = "Nowhere",
                UserId = userId
            }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongTenant_ThrowsInvalidOperation()
    {
        var (options, shipmentId, _, userId) = await SeedShipmentAsync();
        using var ctx = new ShippingDbContext(options);
        var handler = new RecordTrackingEventCommandHandler(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new RecordTrackingEventCommand
            {
                TenantId = Guid.NewGuid(), // different tenant than the seeded shipment
                ShipmentId = shipmentId,
                EventType = TrackingEventType.OrderCreated,
                Location = "Nowhere",
                UserId = userId
            }, CancellationToken.None));
    }
}
