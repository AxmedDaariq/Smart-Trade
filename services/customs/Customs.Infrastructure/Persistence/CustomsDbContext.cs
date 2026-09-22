using Microsoft.EntityFrameworkCore;
using Customs.Domain.Entities;
using Customs.Domain.Entities.Configuration;
using Customs.Domain.Entities.Integration;
using BuildingBlocks.Persistence; // Assuming SmartDbContext is located here

namespace Customs.Infrastructure.Persistence;

public class CustomsDbContext : SmartDbContext
{
    public CustomsDbContext(DbContextOptions<CustomsDbContext> options) : base(options) { }

    // Core Case & Assessment
    public DbSet<CustomsCase> CustomsCases => Set<CustomsCase>();
    public DbSet<CustomsReference> CustomsReferences => Set<CustomsReference>();
    
    public DbSet<CustomsAssessment> CustomsAssessments => Set<CustomsAssessment>();
    public DbSet<DutyAssessment> DutyAssessments => Set<DutyAssessment>();
    public DbSet<TaxAssessment> TaxAssessments => Set<TaxAssessment>();
    public DbSet<CustomsAssessmentLine> CustomsAssessmentLines => Set<CustomsAssessmentLine>();
    
    public DbSet<CustomsValuation> CustomsValuations => Set<CustomsValuation>();
    public DbSet<CustomsValuationComponent> CustomsValuationComponents => Set<CustomsValuationComponent>();
    
    public DbSet<CustomsReassessment> CustomsReassessments => Set<CustomsReassessment>();

    // Risk, Inspection, Holds, and Decisions
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<RiskRuleMatch> RiskRuleMatches => Set<RiskRuleMatch>();
    
    public DbSet<InspectionOrder> InspectionOrders => Set<InspectionOrder>();
    public DbSet<InspectionResult> InspectionResults => Set<InspectionResult>();
    
    public DbSet<CustomsDiscrepancy> CustomsDiscrepancies => Set<CustomsDiscrepancy>();
    public DbSet<CustomsHold> CustomsHolds => Set<CustomsHold>();
    public DbSet<CustomsDecision> CustomsDecisions => Set<CustomsDecision>();
    public DbSet<CustomsClearance> CustomsClearances => Set<CustomsClearance>();
    public DbSet<ReleaseAuthorization> ReleaseAuthorizations => Set<ReleaseAuthorization>();
    
    public DbSet<Exemption> Exemptions => Set<Exemption>();
    public DbSet<ExemptionDecision> ExemptionDecisions => Set<ExemptionDecision>();
    
    // Status History & Audit
    public DbSet<CustomsStatusHistory> CustomsStatusHistories => Set<CustomsStatusHistory>();
    public DbSet<CustomsAuditEntry> CustomsAuditEntries => Set<CustomsAuditEntry>();

    // Configuration
    public DbSet<TariffSchedule> TariffSchedules => Set<TariffSchedule>();
    public DbSet<TariffScheduleVersion> TariffScheduleVersions => Set<TariffScheduleVersion>();
    public DbSet<TariffItem> TariffItems => Set<TariffItem>();
    
    public DbSet<CustomsChargeType> CustomsChargeTypes => Set<CustomsChargeType>();
    
    public DbSet<CustomsRate> CustomsRates => Set<CustomsRate>();
    public DbSet<CustomsRateVersion> CustomsRateVersions => Set<CustomsRateVersion>();
    
    public DbSet<CustomsRule> CustomsRules => Set<CustomsRule>();
    public DbSet<CustomsRuleVersion> CustomsRuleVersions => Set<CustomsRuleVersion>();
    
    public DbSet<ExchangeRateConfiguration> ExchangeRateConfigurations => Set<ExchangeRateConfiguration>();
    
    public DbSet<ExemptionRule> ExemptionRules => Set<ExemptionRule>();
    public DbSet<RiskRule> RiskRules => Set<RiskRule>();
    
    public DbSet<CustomsConfigurationPublication> CustomsConfigurationPublications => Set<CustomsConfigurationPublication>();
    public DbSet<CustomsConfigurationApproval> CustomsConfigurationApprovals => Set<CustomsConfigurationApproval>();
    public DbSet<CustomsConfigurationAudit> CustomsConfigurationAudits => Set<CustomsConfigurationAudit>();

    // Integration
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Core Aggregate
        builder.Entity<CustomsCase>(e =>
        {
            e.HasIndex(c => new { c.TenantId, c.CaseNumber }).IsUnique();
            e.HasIndex(c => c.OrganizationId);
            e.HasIndex(c => new { c.TenantId, c.Status });
            e.HasIndex(c => new { c.TenantId, c.CreatedAtUtc });
            e.HasIndex(c => c.DeclarationId);
            e.HasIndex(c => c.EctnNumber);
            e.Property(c => c.Version).IsRowVersion();
            
            // Restrict cascading delete from Case
            e.HasMany(c => c.Assessments).WithOne(a => a.CustomsCase).HasForeignKey(a => a.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.Valuations).WithOne(v => v.CustomsCase).HasForeignKey(v => v.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.Holds).WithOne(h => h.CustomsCase).HasForeignKey(h => h.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.Discrepancies).WithOne(d => d.CustomsCase).HasForeignKey(d => d.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.Decisions).WithOne(d => d.CustomsCase).HasForeignKey(d => d.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.StatusHistories).WithOne(sh => sh.CustomsCase).HasForeignKey(sh => sh.CustomsCaseId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.References).WithOne(r => r.CustomsCase).HasForeignKey(r => r.CustomsCaseId).OnDelete(DeleteBehavior.Cascade);
        });

        // Assessment
        builder.Entity<CustomsAssessment>(e =>
        {
            e.HasIndex(a => new { a.CustomsCaseId, a.AssessmentVersion }).IsUnique();
            e.HasIndex(a => a.DeclarationId);
            e.HasIndex(a => a.AssessmentNumber);
            e.HasIndex(a => a.Status);
            e.HasIndex(a => a.CalculatedAtUtc);
            
            e.Property(a => a.CustomsValue).HasColumnType("numeric(19,4)");
            e.Property(a => a.DutyAmount).HasColumnType("numeric(19,4)");
            e.Property(a => a.CustomsChargesAmount).HasColumnType("numeric(19,4)");
            e.Property(a => a.TotalAmount).HasColumnType("numeric(19,4)");
            e.Property(a => a.Version).IsRowVersion();

            e.HasMany(a => a.DutyAssessments).WithOne(da => da.CustomsAssessment).HasForeignKey(da => da.CustomsAssessmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(a => a.TaxAssessments).WithOne(ta => ta.CustomsAssessment).HasForeignKey(ta => ta.CustomsAssessmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(a => a.Lines).WithOne(al => al.CustomsAssessment).HasForeignKey(al => al.CustomsAssessmentId).OnDelete(DeleteBehavior.Cascade);
        });

        // Duty Assessment
        builder.Entity<DutyAssessment>(e =>
        {
            e.Property(da => da.Rate).HasColumnType("numeric(19,4)");
            e.Property(da => da.TaxableBase).HasColumnType("numeric(19,4)");
            e.Property(da => da.Quantity).HasColumnType("numeric(19,4)");
            e.Property(da => da.CalculatedAmount).HasColumnType("numeric(19,4)");
            e.Property(da => da.CalculationTraceJson).HasColumnType("jsonb");
        });

        // Tax Assessment
        builder.Entity<TaxAssessment>(e =>
        {
            e.Property(ta => ta.TaxableBase).HasColumnType("numeric(19,4)");
            e.Property(ta => ta.Rate).HasColumnType("numeric(19,4)");
            e.Property(ta => ta.Amount).HasColumnType("numeric(19,4)");
            e.Property(ta => ta.CalculationTraceJson).HasColumnType("jsonb");
        });
        
        // Customs Assessment Line
        builder.Entity<CustomsAssessmentLine>(e =>
        {
            e.HasIndex(l => new { l.CustomsAssessmentId, l.LineNumber }).IsUnique();
            e.Property(l => l.TaxableBase).HasColumnType("numeric(19,4)");
            e.Property(l => l.Rate).HasColumnType("numeric(19,4)");
            e.Property(l => l.Quantity).HasColumnType("numeric(19,4)");
            e.Property(l => l.Amount).HasColumnType("numeric(19,4)");
            e.Property(l => l.CalculationTraceJson).HasColumnType("jsonb");
        });

        // Valuation
        builder.Entity<CustomsValuation>(e =>
        {
            e.Property(v => v.InvoiceValue).HasColumnType("numeric(19,4)");
            e.Property(v => v.FreightValue).HasColumnType("numeric(19,4)");
            e.Property(v => v.InsuranceValue).HasColumnType("numeric(19,4)");
            e.Property(v => v.Additions).HasColumnType("numeric(19,4)");
            e.Property(v => v.Deductions).HasColumnType("numeric(19,4)");
            e.Property(v => v.ExchangeRate).HasColumnType("numeric(19,6)"); // Standard for exchange rates
            e.Property(v => v.ConvertedValue).HasColumnType("numeric(19,4)");
            e.Property(v => v.Version).IsRowVersion();

            e.HasMany(v => v.Components).WithOne(c => c.CustomsValuation).HasForeignKey(c => c.CustomsValuationId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CustomsValuationComponent>(e =>
        {
            e.Property(c => c.Amount).HasColumnType("numeric(19,4)");
        });

        // Configuration - Tariff Schedule
        builder.Entity<TariffScheduleVersion>(e =>
        {
            e.HasIndex(tsv => new { tsv.TariffScheduleId, tsv.VersionNumber }).IsUnique();
            e.Property(tsv => tsv.Version).IsRowVersion();
            
            e.HasMany(tsv => tsv.TariffItems).WithOne(ti => ti.TariffScheduleVersion).HasForeignKey(ti => ti.TariffScheduleVersionId).OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<TariffItem>(e =>
        {
            e.HasIndex(ti => ti.HsCode);
            e.HasIndex(ti => ti.ProcedureCode);
            e.HasIndex(ti => ti.OriginCountryCode);
            e.HasIndex(ti => ti.EffectiveFromUtc);
            e.HasIndex(ti => ti.EffectiveToUtc);
            e.HasIndex(ti => ti.Status);
            
            e.Property(ti => ti.Rate).HasColumnType("numeric(19,4)");
            e.Property(ti => ti.SecondaryRate).HasColumnType("numeric(19,4)");
            e.Property(ti => ti.MinimumAmount).HasColumnType("numeric(19,4)");
            e.Property(ti => ti.MaximumAmount).HasColumnType("numeric(19,4)");
            e.Property(ti => ti.Version).IsRowVersion();
        });

        // Configuration - Rates
        builder.Entity<CustomsRateVersion>(e =>
        {
            e.HasIndex(crv => new { crv.CustomsRateId, crv.VersionNumber }).IsUnique();
            e.Property(crv => crv.Rate).HasColumnType("numeric(19,4)");
            e.Property(crv => crv.MinimumAmount).HasColumnType("numeric(19,4)");
            e.Property(crv => crv.MaximumAmount).HasColumnType("numeric(19,4)");
            e.Property(crv => crv.Version).IsRowVersion();
        });

        // Configuration - Rules
        builder.Entity<CustomsRuleVersion>(e =>
        {
            e.Property(crv => crv.RuleDefinitionJson).HasColumnType("jsonb");
        });
        
        builder.Entity<RiskRule>(e =>
        {
            e.Property(rr => rr.RuleDefinitionJson).HasColumnType("jsonb");
            e.Property(rr => rr.Version).IsRowVersion();
        });
        
        builder.Entity<ExemptionRule>(e =>
        {
            e.Property(er => er.EligibilityDefinitionJson).HasColumnType("jsonb");
            e.Property(er => er.Version).IsRowVersion();
        });

        builder.Entity<ExchangeRateConfiguration>(e =>
        {
            e.Property(erc => erc.Rate).HasColumnType("numeric(19,6)");
        });

        builder.Entity<CustomsConfigurationAudit>(e =>
        {
            e.Property(cca => cca.BeforeJson).HasColumnType("jsonb");
            e.Property(cca => cca.AfterJson).HasColumnType("jsonb");
        });

        // Customs Reference
        builder.Entity<CustomsReference>(e =>
        {
            e.HasIndex(r => new { r.TenantId, r.ReferenceType, r.NormalizedValue });
        });

        // Clearance
        builder.Entity<CustomsClearance>(e =>
        {
            e.HasIndex(c => new { c.CustomsCaseId, c.Status });
            e.HasIndex(c => c.ClearanceNumber);
            e.Property(c => c.Version).IsRowVersion();
            
            e.HasMany(c => c.ReleaseAuthorizations).WithOne(ra => ra.Clearance).HasForeignKey(ra => ra.ClearanceId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReleaseAuthorization>(e =>
        {
            e.HasIndex(ra => new { ra.CustomsCaseId, ra.Status });
            e.HasIndex(ra => ra.AuthorizationNumber);
            e.HasIndex(ra => ra.ExpiresAtUtc);
            e.Property(ra => ra.ConditionsJson).HasColumnType("jsonb");
            e.Property(ra => ra.Version).IsRowVersion();
        });

        // Holds
        builder.Entity<CustomsHold>(e =>
        {
            e.HasIndex(h => new { h.CustomsCaseId, h.Status, h.HoldType });
        });

        // Inspection
        builder.Entity<InspectionOrder>(e =>
        {
            e.HasIndex(i => new { i.CustomsCaseId, i.Status });
            e.HasIndex(i => i.ScheduledAtUtc);
            e.Property(i => i.Version).IsRowVersion();
            
            e.HasMany(io => io.Results).WithOne(ir => ir.InspectionOrder).HasForeignKey(ir => ir.InspectionOrderId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<InspectionResult>(e =>
        {
            e.Property(ir => ir.VerifiedGrossWeight).HasColumnType("numeric(19,4)");
            e.Property(ir => ir.VerifiedNetWeight).HasColumnType("numeric(19,4)");
        });

        // Exemption
        builder.Entity<Exemption>(e =>
        {
            e.Property(ex => ex.RequestedAmount).HasColumnType("numeric(19,4)");
            e.Property(ex => ex.Version).IsRowVersion();
            
            e.HasMany(ex => ex.Decisions).WithOne(ed => ed.Exemption).HasForeignKey(ed => ed.ExemptionId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ExemptionDecision>(e =>
        {
            e.Property(ed => ed.ApprovedAmount).HasColumnType("numeric(19,4)");
        });

        // Risk Assessment
        builder.Entity<RiskAssessment>(e =>
        {
            e.HasIndex(ra => new { ra.CustomsCaseId, ra.RiskLevel });
            e.HasIndex(ra => ra.CalculatedAtUtc);
            
            e.HasMany(ra => ra.RuleMatches).WithOne(rm => rm.RiskAssessment).HasForeignKey(rm => rm.RiskAssessmentId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RiskRuleMatch>(e =>
        {
            e.Property(rm => rm.EvidenceJson).HasColumnType("jsonb");
        });

        // Audit
        builder.Entity<CustomsAuditEntry>(e =>
        {
            e.Property(a => a.BeforeJson).HasColumnType("jsonb");
            e.Property(a => a.AfterJson).HasColumnType("jsonb");
        });

        // Idempotency
        builder.Entity<IdempotencyRecord>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.IdempotencyKey, i.Operation }).IsUnique();
            e.Property(i => i.ResponseBodyJson).HasColumnType("jsonb");
        });

        // Inbox / Outbox
        builder.Entity<InboxMessage>(e =>
        {
            e.HasIndex(i => new { i.MessageId, i.Consumer }).IsUnique();
        });
        
        builder.Entity<OutboxMessage>(e =>
        {
            e.HasIndex(o => o.Status);
            e.HasIndex(o => o.OccurredAtUtc);
            e.Property(o => o.PayloadJson).HasColumnType("jsonb");
        });
    }
}
