using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Tests;

public class RecordTrackingEventCommandTests
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
            Incoterm = Incoterm.FOB,
            Currency = "USD",
            GrossWeightKg = 10000,
            PackageCount = 200,
            UserId = userId
        }, CancellationToken.None);

        return (ctx, shipmentId, tenantId, userId);
    }

    [Fact]
    public async Task Handle_ValidEvent_AddsTrackingEventAndReturnsId()
    {
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, shipmentId, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, _, tenantId, userId) = await SeedShipmentAsync();
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
        var (ctx, shipmentId, _, userId) = await SeedShipmentAsync();
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
