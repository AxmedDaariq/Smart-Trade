using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Api.IntegrationTests;

/// <summary>
/// Persistence integration tests against the real PostgreSQL
/// Testcontainer — unique constraints, tenant isolation queries,
/// navigation includes and the command handlers, all against a
/// real database (no InMemory provider).
/// </summary>
[Collection("shipping-postgres")]
public class ShippingDatabaseIntegrationTests
{
    private readonly PostgresTestContainerFixture _db;

    public ShippingDatabaseIntegrationTests(PostgresTestContainerFixture db)
    {
        _db = db;
    }

    private ShippingDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<ShippingDbContext>()
            .UseNpgsql(_db.ConnectionString)
            .Options);

    private static Shipment CreateShipment(
        Guid tenantId, string shipmentNumber, string shipperReference) => new(
        tenantId: tenantId,
        shipmentNumber: shipmentNumber,
        shipperName: "Acme Exports",
        shipperReference: shipperReference,
        consigneeName: "Global Imports",
        consigneeReference: "GI-001",
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

    private static CreateShipmentCommand CreateCommand(Guid tenantId, Guid userId, string reference) => new()
    {
        TenantId = tenantId,
        UserId = userId,
        ShipperName = "Acme Exports",
        ShipperReference = reference,
        ConsigneeName = "Global Imports",
        ConsigneeReference = "GI-001",
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
        Lines = new()
        {
            new()
            {
                LineNumber = 1,
                Description = "Sesame seeds, hulled",
                HsCode = "1207.40",
                CountryOfOrigin = "SO",
                Quantity = 200,
                UnitOfMeasure = "BAG",
                GrossWeightKg = 10000,
                NetWeightKg = 9000,
                UnitValue = 50,
                TotalValue = 10000,
                Currency = "USD"
            }
        }
    };

    // ── Constraints ──────────────────────────────────────────────

    [Fact]
    public async Task ShipmentNumber_IsUniquePerTenant()
    {
        await using var ctx = CreateContext();
        var tenantId = Guid.NewGuid();

        ctx.Shipments.Add(CreateShipment(tenantId, "SHP-UNIQ-000001", "REF-001"));
        ctx.Shipments.Add(CreateShipment(tenantId, "SHP-UNIQ-000001", "REF-002"));

        // PostgreSQL raises unique_violation (SQLSTATE 23505) on the
        // (tenant_id, shipment_number) unique index.
        await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
    }

    [Fact]
    public async Task SameShipmentNumber_DifferentTenants_IsAllowed()
    {
        await using var ctx = CreateContext();

        ctx.Shipments.Add(CreateShipment(Guid.NewGuid(), "SHP-SHARED-000001", "REF-001"));
        ctx.Shipments.Add(CreateShipment(Guid.NewGuid(), "SHP-SHARED-000001", "REF-002"));

        // Tenant-scoped uniqueness: the same number may exist for another tenant.
        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task ShipmentLine_UniquePerShipmentAndLineNumber()
    {
        await using var ctx = CreateContext();
        var shipment = CreateShipment(Guid.NewGuid(), "SHP-LINE-000001", "REF-001");
        shipment.Lines.Add(new ShipmentLine
        {
            ShipmentId = shipment.Id,
            LineNumber = 1,
            Description = "First line",
            Quantity = 1,
            TotalValue = 10,
            Currency = "USD"
        });
        shipment.Lines.Add(new ShipmentLine
        {
            ShipmentId = shipment.Id,
            LineNumber = 1, // duplicate line number
            Description = "Duplicate line",
            Quantity = 1,
            TotalValue = 10,
            Currency = "USD"
        });
        ctx.Shipments.Add(shipment);

        await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
    }

    // ── Tenant isolation queries ─────────────────────────────────

    [Fact]
    public async Task GetShipmentWithHistoryAsync_IsolatesTenants()
    {
        await using var ctx = CreateContext();
        var tenantId = Guid.NewGuid();
        var shipment = CreateShipment(tenantId, "SHP-TENANT-000001", "REF-001");
        shipment.TransitionStatus(
            ShipmentStatus.Booked, "Booking confirmed", Guid.NewGuid());
        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        var forOwner = await ctx.GetShipmentWithHistoryAsync(
            shipment.Id, tenantId, CancellationToken.None);
        Assert.NotNull(forOwner);
        var history = Assert.Single(forOwner!.StatusHistories);
        Assert.Equal("Draft", history.FromStatus);
        Assert.Equal("Booked", history.ToStatus);

        var forOtherTenant = await ctx.GetShipmentWithHistoryAsync(
            shipment.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.Null(forOtherTenant);
    }

    [Fact]
    public async Task GetShipmentWithTrackingAsync_IncludesTrackingEvents()
    {
        await using var ctx = CreateContext();
        var tenantId = Guid.NewGuid();
        var shipment = CreateShipment(tenantId, "SHP-TRACK-000001", "REF-001");
        shipment.AddTrackingEvent(
            TrackingEventType.CargoPickedUp, "Berbera ICD", "Collected", Guid.NewGuid());
        shipment.AddTrackingEvent(
            TrackingEventType.DepartedOrigin, "Gulf of Aden", null, Guid.NewGuid());
        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        var result = await ctx.GetShipmentWithTrackingAsync(
            shipment.Id, tenantId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result!.TrackingEvents.Count);
        Assert.Equal(TrackingEventType.CargoPickedUp, result.TrackingEvents.First().EventType);
        Assert.Equal("Berbera ICD", result.TrackingEvents.First().Location);
    }

    [Fact]
    public async Task ShipmentExistsAsync_RespectsTenantBoundary()
    {
        await using var ctx = CreateContext();
        var tenantId = Guid.NewGuid();
        var shipment = CreateShipment(tenantId, "SHP-EXIST-000001", "REF-001");
        ctx.Shipments.Add(shipment);
        await ctx.SaveChangesAsync();

        Assert.True(await ctx.ShipmentExistsAsync(shipment.Id, tenantId, CancellationToken.None));
        Assert.False(await ctx.ShipmentExistsAsync(shipment.Id, Guid.NewGuid(), CancellationToken.None));
        Assert.False(await ctx.ShipmentExistsAsync(Guid.NewGuid(), tenantId, CancellationToken.None));
    }

    // ── Handlers against a real database ──────────────────────────

    [Fact]
    public async Task CreateShipmentHandler_PersistsShipmentWithLines()
    {
        await using var ctx = CreateContext();
        var handler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());
        var tenantId = Guid.NewGuid();

        var shipmentId = await handler.Handle(
            CreateCommand(tenantId, Guid.NewGuid(), "REF-HANDLER-001"), CancellationToken.None);

        await using var verify = CreateContext();
        var saved = await verify.Shipments
            .Include(s => s.Lines)
            .FirstAsync(s => s.Id == shipmentId);

        Assert.Equal(tenantId, saved.TenantId);
        Assert.StartsWith("SHP-", saved.ShipmentNumber);
        Assert.Equal(ShipmentStatus.Draft, saved.Status);
        var line = Assert.Single(saved.Lines);
        Assert.Equal("1207.40", line.HsCode);
        Assert.Equal(shipmentId, line.ShipmentId);
    }

    [Fact]
    public async Task RecordTrackingEventHandler_PersistsEventAndAutoTransitionsStatus()
    {
        await using var ctx = CreateContext();
        var createHandler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = await createHandler.Handle(
            CreateCommand(tenantId, userId, "REF-TRACK-001"), CancellationToken.None);

        var handler = new RecordTrackingEventCommandHandler(ctx);
        var eventId = await handler.Handle(new RecordTrackingEventCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            EventType = TrackingEventType.CargoPickedUp,
            Location = "Berbera ICD",
            Latitude = 10.4,
            Longitude = 45.0,
            VehicleIdentifier = "TRK-7788",
            CarrierEventCode = "PUP-001",
            UserId = userId
        }, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, eventId);

        await using var verify = CreateContext();
        var shipment = await verify.Shipments
            .Include(s => s.TrackingEvents)
            .Include(s => s.StatusHistories)
            .FirstAsync(s => s.Id == shipmentId);

        // Auto-transition: CargoPickedUp → PickedUp, with history.
        Assert.Equal(ShipmentStatus.PickedUp, shipment.Status);
        var trackingEvent = Assert.Single(shipment.TrackingEvents);
        Assert.Equal(eventId, trackingEvent.Id);
        Assert.Equal(10.4, trackingEvent.Latitude);
        Assert.Equal(45.0, trackingEvent.Longitude);
        Assert.Equal("TRK-7788", trackingEvent.VehicleIdentifier);
        Assert.Equal("PUP-001", trackingEvent.CarrierEventCode);
        var history = Assert.Single(shipment.StatusHistories);
        Assert.Equal("Draft", history.FromStatus);
        Assert.Equal("PickedUp", history.ToStatus);
    }

    [Fact]
    public async Task ConfirmCustomsClearanceHandler_PersistsClearanceAndLinksCase()
    {
        await using var ctx = CreateContext();
        var createHandler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = await createHandler.Handle(
            CreateCommand(tenantId, userId, "REF-CUSTOMS-001"), CancellationToken.None);

        var customsCaseId = Guid.NewGuid();
        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);
        await handler.Handle(new ConfirmCustomsClearanceCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            CustomsCaseId = customsCaseId,
            CustomsCaseNumber = "CUS-IT-2026-0001",
            UserId = userId
        }, CancellationToken.None);

        await using var verify = CreateContext();
        var shipment = await verify.Shipments
            .Include(s => s.TrackingEvents)
            .Include(s => s.StatusHistories)
            .FirstAsync(s => s.Id == shipmentId);

        Assert.True(shipment.IsCustomsCleared);
        Assert.Equal(customsCaseId, shipment.CustomsCaseId);
        Assert.Equal("CUS-IT-2026-0001", shipment.CustomsCaseNumber);
        Assert.NotNull(shipment.CustomsClearedAtUtc);
        Assert.Equal(ShipmentStatus.CustomsCleared, shipment.Status);

        var history = Assert.Single(shipment.StatusHistories);
        Assert.Equal("CustomsCleared", history.ToStatus);
        var trackingEvent = Assert.Single(shipment.TrackingEvents);
        Assert.Equal(TrackingEventType.CustomsCleared, trackingEvent.EventType);
    }

    [Fact]
    public async Task ConfirmCustomsClearanceHandler_AlreadyCleared_Throws()
    {
        await using var ctx = CreateContext();
        var createHandler = new CreateShipmentCommandHandler(ctx, new ShipmentNumberGenerator());
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shipmentId = await createHandler.Handle(
            CreateCommand(tenantId, userId, "REF-CUSTOMS-002"), CancellationToken.None);

        var handler = new ConfirmCustomsClearanceCommandHandler(ctx);
        await handler.Handle(new ConfirmCustomsClearanceCommand
        {
            TenantId = tenantId,
            ShipmentId = shipmentId,
            CustomsCaseId = Guid.NewGuid(),
            CustomsCaseNumber = "CUS-IT-2026-0003",
            UserId = userId
        }, CancellationToken.None);

        // Second clearance attempt on the same shipment must fail.
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ConfirmCustomsClearanceCommand
            {
                TenantId = tenantId,
                ShipmentId = shipmentId,
                CustomsCaseId = Guid.NewGuid(),
                CustomsCaseNumber = "CUS-IT-2026-0004",
                UserId = userId
            }, CancellationToken.None));
    }
}
