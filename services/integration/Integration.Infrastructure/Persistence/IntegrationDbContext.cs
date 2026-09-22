using Microsoft.EntityFrameworkCore;
using Integration.Domain.Entities;

namespace Integration.Infrastructure.Persistence;

public class IntegrationDbContext : DbContext
{
    // 1. Integration Core
    public DbSet<IntegrationPartner> Partners { get; set; } = null!;
    public DbSet<IntegrationSystem> Systems { get; set; } = null!;
    public DbSet<IntegrationEndpoint> Endpoints { get; set; } = null!;
    public DbSet<IntegrationAuthentication> Authentications { get; set; } = null!;
    public DbSet<IntegrationApiVersion> ApiVersions { get; set; } = null!;

    // 2. Events & Contracts
    public DbSet<IntegrationEventContract> EventContracts { get; set; } = null!;
    public DbSet<IntegrationEventSubscription> EventSubscriptions { get; set; } = null!;
    public DbSet<IntegrationEventDelivery> EventDeliveries { get; set; } = null!;
    public DbSet<IntegrationMessage> Messages { get; set; } = null!;

    // 3. API & Webhooks
    public DbSet<IntegrationRequest> Requests { get; set; } = null!;
    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; } = null!;
    public DbSet<WebhookDelivery> WebhookDeliveries { get; set; } = null!;
    public DbSet<WebhookInboundEvent> WebhookInboundEvents { get; set; } = null!;

    // 4. Resiliency & Policy
    public DbSet<IntegrationMapping> Mappings { get; set; } = null!;
    public DbSet<IntegrationRetryPolicy> RetryPolicies { get; set; } = null!;
    public DbSet<IntegrationDeadLetter> DeadLetters { get; set; } = null!;
    public DbSet<IntegrationIdempotencyRecord> IdempotencyRecords { get; set; } = null!;
    public DbSet<IntegrationRateLimitPolicy> RateLimitPolicies { get; set; } = null!;
    public DbSet<IntegrationCircuitState> CircuitStates { get; set; } = null!;

    // 5. Operations & Health
    public DbSet<IntegrationHealthRecord> HealthRecords { get; set; } = null!;
    public DbSet<IntegrationAlert> Alerts { get; set; } = null!;
    public DbSet<IntegrationJob> Jobs { get; set; } = null!;
    public DbSet<IntegrationFileExchange> FileExchanges { get; set; } = null!;

    // 6. Verification & Transformation
    public DbSet<IntegrationSchemaValidation> SchemaValidations { get; set; } = null!;
    public DbSet<IntegrationTransformationRecord> TransformationRecords { get; set; } = null!;

    // 7. Integrations & Platform Base
    public DbSet<IntegrationExternalReference> ExternalReferences { get; set; } = null!;
    public DbSet<IntegrationAuditEntry> AuditEntries { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;

    public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Integration Core
        modelBuilder.Entity<IntegrationPartner>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PartnerReference }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<IntegrationSystem>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemReference }).IsUnique();
            e.HasIndex(x => new { x.PartnerId, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<IntegrationEndpoint>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.EndpointReference }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<IntegrationAuthentication>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.AuthenticationReference }).IsUnique();
        });

        modelBuilder.Entity<IntegrationApiVersion>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.Version }).IsUnique();
        });

        // 2. Events & Contracts
        modelBuilder.Entity<IntegrationEventContract>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ContractReference, x.SchemaVersion }).IsUnique();
            e.Property(x => x.SchemaJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IntegrationEventSubscription>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ContractId, x.SubscriberSystem });
        });

        modelBuilder.Entity<IntegrationEventDelivery>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.MessageId, x.EventType, x.TargetSystem });
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.OccurredAtUtc);
        });

        modelBuilder.Entity<IntegrationMessage>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.MessageId, x.Direction });
            e.HasIndex(x => x.Status);
        });

        // 3. API & Webhooks
        modelBuilder.Entity<IntegrationRequest>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.EndpointId, x.CorrelationId });
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.RequestTimestampUtc);
        });

        modelBuilder.Entity<WebhookSubscription>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PartnerId, x.WebhookReference }).IsUnique();
            e.HasIndex(x => x.EventType);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<WebhookDelivery>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.WebhookSubscriptionId, x.EventId });
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<WebhookInboundEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PartnerId, x.ProviderEventId }).IsUnique();
            e.HasIndex(x => x.Status);
        });

        // 4. Resiliency & Policy
        modelBuilder.Entity<IntegrationMapping>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.MappingReference }).IsUnique();
            e.Property(x => x.ConfigurationJson).HasColumnType("jsonb");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<IntegrationRetryPolicy>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PolicyReference }).IsUnique();
            e.Property(x => x.RetryableStatusCodesJson).HasColumnType("jsonb");
            e.Property(x => x.RetryableErrorCodesJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IntegrationDeadLetter>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Status, x.CreatedAtUtc });
            e.HasIndex(x => new { x.TenantId, x.TargetSystem, x.Status });
        });

        modelBuilder.Entity<IntegrationIdempotencyRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.IdempotencyKey, x.Operation }).IsUnique();
            e.Property(x => x.ResponseBodyJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IntegrationRateLimitPolicy>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.EndpointId }).IsUnique();
        });

        modelBuilder.Entity<IntegrationCircuitState>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.EndpointId }).IsUnique();
        });

        // 5. Operations & Health
        modelBuilder.Entity<IntegrationHealthRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SystemId, x.EndpointId, x.CheckedAtUtc });
            e.Property(x => x.DetailsJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IntegrationAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Severity, x.Status, x.OccurredAtUtc });
        });

        modelBuilder.Entity<IntegrationJob>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.JobType, x.Status });
        });

        modelBuilder.Entity<IntegrationFileExchange>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PartnerId, x.FileReference }).IsUnique();
            e.HasIndex(x => x.Status);
        });

        // 6. Verification & Transformation
        modelBuilder.Entity<IntegrationSchemaValidation>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.MessageId, x.ValidationDirection });
            e.Property(x => x.ValidationErrorsJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IntegrationTransformationRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.MessageId, x.MappingId });
        });

        // 7. Integrations & Platform Base
        modelBuilder.Entity<IntegrationExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.InternalSystem, x.InternalResourceType, x.InternalResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });

        modelBuilder.Entity<IntegrationAuditEntry>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.Property(x => x.BeforeJson).HasColumnType("jsonb");
            e.Property(x => x.AfterJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<InboxMessage>(e =>
        {
            e.HasIndex(i => new { i.MessageId, i.Consumer }).IsUnique();
        });
        
        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.HasIndex(o => o.Status);
            e.HasIndex(o => o.OccurredAtUtc);
            e.Property(o => o.PayloadJson).HasColumnType("jsonb");
        });
    }
}
