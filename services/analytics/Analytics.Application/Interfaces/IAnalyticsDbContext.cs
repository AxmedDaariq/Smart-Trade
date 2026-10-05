using System;
using System.Threading;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Application.Interfaces;

/// <summary>
/// Persistence abstraction for the Analytics Application layer.
///
/// Declared here (and implemented by <c>Analytics.Infrastructure.Persistence.AnalyticsDbContext</c>)
/// so the Application layer never takes a dependency on Infrastructure or EF Core,
/// which would otherwise create a circular project reference.
/// </summary>
public interface IAnalyticsDbContext
{
    /// <summary>Loads a report definition by its identifier, or <c>null</c> when it does not exist.</summary>
    Task<ReportDefinition?> FindReportDefinitionAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Idempotency probe: has this operational event already been ingested?</summary>
    Task<bool> FactTripMilestoneExistsAsync(Guid sourceEventId, CancellationToken cancellationToken = default);

    /// <summary>Stages a trip milestone fact for persistence.</summary>
    void AddFactTripMilestone(FactTripMilestone fact);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}