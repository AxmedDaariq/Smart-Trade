using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    // 1. Notification Core & Recipients
    public DbSet<Domain.Entities.Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationRecipient> Recipients { get; set; } = null!;

    // 2. Channels
    public DbSet<NotificationChannel> Channels { get; set; } = null!;
    public DbSet<EmailNotification> EmailNotifications { get; set; } = null!;
    public DbSet<SmsNotification> SmsNotifications { get; set; } = null!;
    public DbSet<PushNotification> PushNotifications { get; set; } = null!;
    public DbSet<InAppNotification> InAppNotifications { get; set; } = null!;

    // 3. Templates & Rules
    public DbSet<NotificationTemplate> Templates { get; set; } = null!;
    public DbSet<NotificationTemplateVersion> TemplateVersions { get; set; } = null!;
    public DbSet<NotificationTemplateApproval> TemplateApprovals { get; set; } = null!;
    public DbSet<NotificationRule> Rules { get; set; } = null!;

    // 4. Preferences & Quiet Hours
    public DbSet<NotificationPreference> Preferences { get; set; } = null!;
    public DbSet<NotificationChannelPreference> ChannelPreferences { get; set; } = null!;

    // 5. Delivery & Status
    public DbSet<NotificationRequest> Requests { get; set; } = null!;
    public DbSet<NotificationDelivery> Deliveries { get; set; } = null!;
    public DbSet<NotificationDeliveryAttempt> DeliveryAttempts { get; set; } = null!;
    public DbSet<NotificationReadState> ReadStates { get; set; } = null!;
    public DbSet<NotificationStatusHistory> StatusHistories { get; set; } = null!;

    // 6. Provider Integrations
    public DbSet<NotificationProviderCallback> ProviderCallbacks { get; set; } = null!;
    public DbSet<NotificationProviderReference> ProviderReferences { get; set; } = null!;

    // 7. Scheduling & Broadcast
    public DbSet<NotificationSchedule> Schedules { get; set; } = null!;
    public DbSet<NotificationBroadcast> Broadcasts { get; set; } = null!;
    public DbSet<NotificationBroadcastRecipient> BroadcastRecipients { get; set; } = null!;

    // 8. Resiliency & Audit
    public DbSet<NotificationRetryPolicy> RetryPolicies { get; set; } = null!;
    public DbSet<NotificationDeadLetter> DeadLetters { get; set; } = null!;
    public DbSet<NotificationIdempotencyRecord> IdempotencyRecords { get; set; } = null!;
    public DbSet<NotificationAlert> Alerts { get; set; } = null!;
    public DbSet<NotificationAuditEntry> AuditEntries { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;

    // 9. Attachments & Escalation
    public DbSet<NotificationEscalation> Escalations { get; set; } = null!;
    public DbSet<NotificationAttachment> Attachments { get; set; } = null!;

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Notification
        modelBuilder.Entity<Domain.Entities.Notification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.Status });
            e.HasIndex(x => new { x.TenantId, x.NotificationType });
            e.HasIndex(x => new { x.TenantId, x.Priority });
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => x.ScheduledAtUtc);
            e.HasIndex(x => x.ExpiresAtUtc);
            e.HasIndex(x => x.SourceService);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<NotificationRecipient>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.UserReference });
            e.HasIndex(x => new { x.TenantId, x.UserReference, x.ReadAtUtc });
        });

        // 2. Channels
        modelBuilder.Entity<NotificationChannel>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.RecipientId, x.ChannelType });
        });

        modelBuilder.Entity<EmailNotification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId });
        });
        
        modelBuilder.Entity<SmsNotification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId });
        });
        
        modelBuilder.Entity<PushNotification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId });
            e.Property(x => x.PayloadJson).HasColumnType("jsonb");
        });
        
        modelBuilder.Entity<InAppNotification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.RecipientId, x.IsRead });
        });

        // 3. Templates & Rules
        modelBuilder.Entity<NotificationTemplate>(e =>
        {
            // Note: TenantId can be null for global templates, so we don't index as composite unique trivially unless we handle nulls.
            e.HasIndex(x => new { x.TemplateReference, x.TenantId }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.NotificationType, x.ChannelType, x.LanguageCode, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<NotificationTemplateVersion>(e =>
        {
            e.HasIndex(x => new { x.TemplateId, x.VersionNumber }).IsUnique();
            e.Property(x => x.VariablesSchemaJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<NotificationRule>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RuleReference }).IsUnique();
            e.Property(x => x.ConditionsJson).HasColumnType("jsonb");
            e.Property(x => x.Version).IsRowVersion();
        });

        // 4. Preferences
        modelBuilder.Entity<NotificationPreference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.UserReference, x.NotificationType }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<NotificationChannelPreference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.UserReference, x.NotificationType, x.ChannelType }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        // 5. Delivery
        modelBuilder.Entity<NotificationRequest>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RequestReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.IdempotencyKey });
        });

        modelBuilder.Entity<NotificationDelivery>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.ChannelId });
            e.HasIndex(x => new { x.TenantId, x.Status });
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => x.LastAttemptAtUtc);
        });

        modelBuilder.Entity<NotificationDeliveryAttempt>(e =>
        {
            e.HasIndex(x => new { x.DeliveryId, x.AttemptNumber }).IsUnique();
            e.HasIndex(x => x.AttemptedAtUtc);
        });

        modelBuilder.Entity<NotificationReadState>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.UserReference }).IsUnique();
        });

        modelBuilder.Entity<NotificationStatusHistory>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.OccurredAtUtc });
        });

        // 6. Providers
        modelBuilder.Entity<NotificationProviderCallback>(e =>
        {
            e.HasIndex(x => new { x.ProviderName, x.ProviderEventId }).IsUnique();
            e.HasIndex(x => x.ReceivedAtUtc);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<NotificationProviderReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ChannelType, x.ProviderName }).IsUnique();
        });

        // 7. Scheduling & Broadcast
        modelBuilder.Entity<NotificationSchedule>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId }).IsUnique();
            e.HasIndex(x => new { x.Status, x.NextRunAtUtc });
        });

        modelBuilder.Entity<NotificationBroadcast>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.BroadcastReference }).IsUnique();
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<NotificationBroadcastRecipient>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.BroadcastId, x.UserReference }).IsUnique();
        });

        // 8. Resiliency & Audit
        modelBuilder.Entity<NotificationIdempotencyRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.IdempotencyKey, x.Operation }).IsUnique();
            e.Property(x => x.ResponseBodyJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<NotificationDeadLetter>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Status, x.CreatedAtUtc });
        });

        modelBuilder.Entity<NotificationAuditEntry>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.OccurredAtUtc);
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

        // 9. Attachments & Escalation
        modelBuilder.Entity<NotificationEscalation>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.Level }).IsUnique();
        });

        modelBuilder.Entity<NotificationAttachment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NotificationId, x.StorageKey }).IsUnique();
        });
    }
}
