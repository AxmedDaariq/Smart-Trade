using Microsoft.EntityFrameworkCore;
using SmartTrade.Analytics.Domain.Entities;

namespace SmartTrade.Analytics.Infrastructure.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    public DbSet<DimTenant> DimTenants => Set<DimTenant>();
    public DbSet<DimDate> DimDates => Set<DimDate>();
    public DbSet<DimVehicle> DimVehicles => Set<DimVehicle>();
    public DbSet<DimCorridor> DimCorridors => Set<DimCorridor>();

    public DbSet<FactTrip> FactTrips => Set<FactTrip>();
    public DbSet<FactPaymentOperational> FactPayments => Set<FactPaymentOperational>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("smart_analytics");

        // DimTenant
        modelBuilder.Entity<DimTenant>().HasKey(x => x.TenantKey);
        modelBuilder.Entity<DimTenant>().HasIndex(x => x.TenantId);

        // FactTrip
        modelBuilder.Entity<FactTrip>().HasKey(x => x.TripKey);
        modelBuilder.Entity<FactTrip>().Property(x => x.PlannedDistance).HasColumnType("numeric(18,2)");
        modelBuilder.Entity<FactTrip>().Property(x => x.ActualDistance).HasColumnType("numeric(18,2)");
        modelBuilder.Entity<FactTrip>().Property(x => x.PlannedDuration).HasColumnType("numeric(18,2)");
        modelBuilder.Entity<FactTrip>().Property(x => x.ActualDuration).HasColumnType("numeric(18,2)");
        modelBuilder.Entity<FactTrip>().Property(x => x.DelayMinutes).HasColumnType("numeric(18,2)");
        
        modelBuilder.Entity<FactTrip>()
            .HasIndex(x => new { x.TenantKey, x.DateKey });

        // FactPaymentOperational
        modelBuilder.Entity<FactPaymentOperational>().HasKey(x => x.PaymentKey);
        modelBuilder.Entity<FactPaymentOperational>().Property(x => x.TotalAmount).HasColumnType("numeric(18,4)");
        modelBuilder.Entity<FactPaymentOperational>().Property(x => x.RefundAmount).HasColumnType("numeric(18,4)");
        modelBuilder.Entity<FactPaymentOperational>().Property(x => x.SettlementAmount).HasColumnType("numeric(18,4)");
    }
}
