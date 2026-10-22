using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Persistence;

public abstract class SmartDbContext<TContext> : DbContext
    where TContext : DbContext
{
    protected SmartDbContext(DbContextOptions<TContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Enforce Snake Case convention globally if needed
        // For Postgres, it is standard. Npgsql provides a method for this, 
        // typically configured via options.UseSnakeCaseNamingConvention() in Program.cs.
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var utcNow = DateTime.UtcNow;

            var createdAtProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAtUtc" || p.Metadata.Name == "created_at_utc");
            if (createdAtProp != null && entry.State == EntityState.Added)
            {
                createdAtProp.CurrentValue = utcNow;
            }

            var updatedAtProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAtUtc" || p.Metadata.Name == "updated_at_utc");
            if (updatedAtProp != null)
            {
                updatedAtProp.CurrentValue = utcNow;
            }
        }
    }
}
