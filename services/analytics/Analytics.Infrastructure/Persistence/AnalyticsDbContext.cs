using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Analytics.Application.Interfaces;
using Analytics.Domain.Entities;

namespace Analytics.Infrastructure.Persistence;

public class AnalyticsDbContext : DbContext, IAnalyticsDbContext
{
    public DbSet<DimTenant> DimTenants { get; set; } = null!;
    public DbSet<FactTripMilestone> FactTripMilestones { get; set; } = null!;
    public DbSet<KpiDefinition> KpiDefinitions { get; set; } = null!;
    public DbSet<ReportDefinition> ReportDefinitions { get; set; } = null!;

    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    // ── IAnalyticsDbContext (Application-layer abstraction) ───────────────────

    public Task<ReportDefinition?> FindReportDefinitionAsync(Guid id, CancellationToken cancellationToken = default)
        => ReportDefinitions.FindAsync(new object[] { id }, cancellationToken).AsTask();

    public Task<bool> FactTripMilestoneExistsAsync(Guid sourceEventId, CancellationToken cancellationToken = default)
        => FactTripMilestones.AnyAsync(f => f.SourceEventId == sourceEventId, cancellationToken);

    public void AddFactTripMilestone(FactTripMilestone fact)
        => FactTripMilestones.Add(fact);

    public async Task<double> GetAverageTransitTimeAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var query = FactTripMilestones.AsQueryable();
        if (tenantId != Guid.Empty)
        {
            query = query.Where(f => f.TenantId == tenantId);
        }

        var hasRecords = await query.AnyAsync(f => f.ActualDurationFromStart.HasValue, cancellationToken);
        if (!hasRecords) return 0.0;

        return await query
            .Where(f => f.ActualDurationFromStart.HasValue)
            .AverageAsync(f => f.ActualDurationFromStart!.Value.TotalHours, cancellationToken);
    }

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
