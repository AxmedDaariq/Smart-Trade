using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands;
using Shipping.Domain.Entities;
using Shipping.Domain.Entities.Integration;
using Shipping.Domain.Enums;

namespace Shipping.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Shipping bounded context.
/// Inherits SmartDbContext from BuildingBlocks.Persistence for
/// multi-tenancy, soft-delete, audit, and concurrency conventions.
/// </summary>
public class ShippingDbContext : SmartDbContext<ShippingDbContext>,
    IShippingDbContext,
    IShippingQueryContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options) { }

    // ── Core ─────────────────────────────────────────────────────────────────
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentLine> ShipmentLines => Set<ShipmentLine>();
    public DbSet<ShipmentContainer> ShipmentContainers => Set<ShipmentContainer>();
    public DbSet<ShipmentDocument> ShipmentDocuments => Set<ShipmentDocument>();

    // ── Tracking ─────────────────────────────────────────────────────────────
    public DbSet<TrackingEvent> TrackingEvents => Set<TrackingEvent>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();

    // ── Integration ───────────────────────────────────────────────────────────
    public DbSet<ShippingOutboxMessage> OutboxMessages => Set<ShippingOutboxMessage>();
    public DbSet<ShippingInboxMessage> InboxMessages => Set<ShippingInboxMessage>();
    public DbSet<ShippingIdempotencyRecord> IdempotencyRecords => Set<ShippingIdempotencyRecord>();

    // ── IShippingDbContext ────────────────────────────────────────────────────
    public void AddShipment(Shipment shipment) => Shipments.Add(shipment);

    // ── IShippingQueryContext ─────────────────────────────────────────────────
    public async Task<Shipment?> GetShipmentWithHistoryAsync(
        Guid shipmentId, Guid tenantId, CancellationToken cancellationToken) =>
        await Shipments
            .Include(s => s.StatusHistories)
            .FirstOrDefaultAsync(s => s.Id == shipmentId && s.TenantId == tenantId, cancellationToken);

    public async Task<Shipment?> GetShipmentWithTrackingAsync(
        Guid shipmentId, Guid tenantId, CancellationToken cancellationToken) =>
        await Shipments
            .Include(s => s.StatusHistories)
            .Include(s => s.TrackingEvents)
            .FirstOrDefaultAsync(s => s.Id == shipmentId && s.TenantId == tenantId, cancellationToken);

    public async Task<bool> ShipmentExistsAsync(
        Guid shipmentId, Guid tenantId, CancellationToken cancellationToken) =>
        await Shipments.AnyAsync(s => s.Id == shipmentId && s.TenantId == tenantId, cancellationToken);

    // ── Queryable (for controllers/read projections) ───────────────────────────
    public IQueryable<Shipment> ShipmentsQuery => Shipments;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Shipment ─────────────────────────────────────────────────────────
        builder.Entity<Shipment>(e =>
        {
            e.HasIndex(s => new { s.TenantId, s.ShipmentNumber }).IsUnique();
            e.HasIndex(s => s.OrganizationId);
            e.HasIndex(s => new { s.TenantId, s.Status });
            e.HasIndex(s => new { s.TenantId, s.CreatedAtUtc });
            e.HasIndex(s => s.CarrierBookingReference);
            e.HasIndex(s => s.MasterBillNumber);
            e.HasIndex(s => s.HouseBillNumber);
            e.HasIndex(s => s.CustomsCaseId);

            e.Property(s => s.GrossWeightKg).HasColumnType("numeric(19,4)");
            e.Property(s => s.NetWeightKg).HasColumnType("numeric(19,4)");
            e.Property(s => s.VolumeM3).HasColumnType("numeric(19,4)");
            e.Property(s => s.FreightValue).HasColumnType("numeric(19,4)");
            e.Property(s => s.InsuranceValue).HasColumnType("numeric(19,4)");
            e.Property(s => s.DeclaredValue).HasColumnType("numeric(19,4)");
            e.Property(s => s.Version).IsRowVersion();

            e.Property(s => s.TransportMode).HasConversion<string>();
            e.Property(s => s.CargoType).HasConversion<string>();
            e.Property(s => s.Incoterm).HasConversion<string>();
            e.Property(s => s.Status).HasConversion<string>();

            e.HasMany(s => s.Lines)
                .WithOne(l => l.Shipment)
                .HasForeignKey(l => l.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(s => s.Containers)
                .WithOne(c => c.Shipment)
                .HasForeignKey(c => c.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(s => s.TrackingEvents)
                .WithOne(t => t.Shipment)
                .HasForeignKey(t => t.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(s => s.Documents)
                .WithOne(d => d.Shipment)
                .HasForeignKey(d => d.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(s => s.StatusHistories)
                .WithOne(h => h.Shipment)
                .HasForeignKey(h => h.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ShipmentLine ─────────────────────────────────────────────────────
        builder.Entity<ShipmentLine>(e =>
        {
            e.HasIndex(l => new { l.ShipmentId, l.LineNumber }).IsUnique();
            e.Property(l => l.Quantity).HasColumnType("numeric(19,4)");
            e.Property(l => l.GrossWeightKg).HasColumnType("numeric(19,4)");
            e.Property(l => l.NetWeightKg).HasColumnType("numeric(19,4)");
            e.Property(l => l.VolumeM3).HasColumnType("numeric(19,4)");
            e.Property(l => l.UnitValue).HasColumnType("numeric(19,4)");
            e.Property(l => l.TotalValue).HasColumnType("numeric(19,4)");
        });

        // ── ShipmentContainer ─────────────────────────────────────────────────
        builder.Entity<ShipmentContainer>(e =>
        {
            e.HasIndex(c => c.ContainerNumber);
            e.Property(c => c.TareWeightKg).HasColumnType("numeric(19,4)");
            e.Property(c => c.CargoWeightKg).HasColumnType("numeric(19,4)");
            e.Property(c => c.VerifiedGrossMassKg).HasColumnType("numeric(19,4)");
            e.Property(c => c.SetTemperatureCelsius).HasColumnType("numeric(5,2)");
        });

        // ── TrackingEvent ─────────────────────────────────────────────────────
        builder.Entity<TrackingEvent>(e =>
        {
            e.HasIndex(t => new { t.ShipmentId, t.OccurredAtUtc });
            e.HasIndex(t => t.EventType);
            e.Property(t => t.EventType).HasConversion<string>();
        });

        // ── ShipmentStatusHistory ─────────────────────────────────────────────
        builder.Entity<ShipmentStatusHistory>(e =>
        {
            e.HasIndex(h => new { h.ShipmentId, h.ChangedAtUtc });
        });

        // ── ShipmentDocument ─────────────────────────────────────────────────
        builder.Entity<ShipmentDocument>(e =>
        {
            e.HasIndex(d => new { d.ShipmentId, d.DocumentType });
            e.HasIndex(d => d.DocumentNumber);
        });

        // ── Outbox ────────────────────────────────────────────────────────────
        builder.Entity<ShippingOutboxMessage>(e =>
        {
            e.HasIndex(o => o.Status);
            e.HasIndex(o => o.OccurredAtUtc);
            e.Property(o => o.PayloadJson).HasColumnType("jsonb");
        });

        // ── Inbox ─────────────────────────────────────────────────────────────
        builder.Entity<ShippingInboxMessage>(e =>
        {
            e.HasIndex(i => new { i.MessageId, i.Consumer }).IsUnique();
        });

        // ── Idempotency ───────────────────────────────────────────────────────
        builder.Entity<ShippingIdempotencyRecord>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.IdempotencyKey, i.Operation }).IsUnique();
            e.Property(i => i.ResponseBodyJson).HasColumnType("jsonb");
        });
    }
}
