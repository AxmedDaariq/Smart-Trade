using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Persistence;

public class AnalyticsDbContext : DbContext
{
    public DbSet<DimTenant> DimTenants { get; set; }
    public DbSet<FactTripMilestone> FactTripMilestones { get; set; }
    public DbSet<KpiDefinition> KpiDefinitions { get; set; }
    public DbSet<ReportDefinition> ReportDefinitions { get; set; }

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // PostGIS setup would normally happen here if we had spatial entities in this snippet (e.g. FactGpsEvent with Point geography)
        // modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<FactTripMilestone>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.OccurredAt }); // Frequent filtering pattern
            entity.HasIndex(e => e.SourceEventId).IsUnique(); // Idempotency check against double ingestion
        });

        modelBuilder.Entity<KpiDefinition>(entity =>
        {
            entity.HasKey(e => e.Code); // Code is the primary identifier for KPIs
        });
        
        modelBuilder.Entity<ReportDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Global reports + Tenant specific ones
        });
    }
}
