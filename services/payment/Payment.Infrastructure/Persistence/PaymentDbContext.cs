using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Domain.Entities.Billing;
using Payment.Domain.Entities.Configuration;
using Payment.Domain.Entities.Integration;
using BuildingBlocks.Persistence; // Base SmartDbContext

namespace Payment.Infrastructure.Persistence;

public class PaymentDbContext : SmartDbContext<PaymentDbContext>
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    // Core
    public DbSet<PaymentIntent> PaymentIntents => Set<PaymentIntent>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    
    // Billing
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    
    // Configuration
    public DbSet<PaymentGatewayConfiguration> PaymentGatewayConfigurations => Set<PaymentGatewayConfiguration>();
    public DbSet<PaymentMethodConfiguration> PaymentMethodConfigurations => Set<PaymentMethodConfiguration>();
    
    // Integration
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Payment Intent
        builder.Entity<PaymentIntent>(e =>
        {
            e.HasIndex(pi => new { pi.TenantId, pi.IntentReference }).IsUnique();
            e.HasIndex(pi => new { pi.SourceSystem, pi.SourceReferenceId });
            e.HasIndex(pi => pi.Status);
            e.HasIndex(pi => pi.CreatedAtUtc);
            
            e.Property(pi => pi.AmountRequested).HasColumnType("numeric(19,4)");
            e.Property(pi => pi.MetadataJson).HasColumnType("jsonb");
            e.Property(pi => pi.Version).IsRowVersion();
            
            e.HasMany(pi => pi.Transactions)
             .WithOne(t => t.PaymentIntent)
             .HasForeignKey(t => t.PaymentIntentId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment Transaction
        builder.Entity<PaymentTransaction>(e =>
        {
            e.HasIndex(pt => pt.GatewayTransactionId);
            e.HasIndex(pt => pt.Status);
            
            e.Property(pt => pt.Amount).HasColumnType("numeric(19,4)");
            e.Property(pt => pt.PaymentMethodDetailsJson).HasColumnType("jsonb");
            e.Property(pt => pt.Version).IsRowVersion();
        });

        // Ledger Entry (Immutable)
        builder.Entity<LedgerEntry>(e =>
        {
            e.HasIndex(le => new { le.TenantId, le.CustomerAccountId });
            e.HasIndex(le => le.PostedAtUtc);
            e.HasIndex(le => le.Hash).IsUnique();
            
            e.Property(le => le.Amount).HasColumnType("numeric(19,4)");
            // Ledger entries should never be updated or cascade deleted
        });

        // Billing
        builder.Entity<CustomerAccount>(e =>
        {
            e.HasIndex(ca => new { ca.TenantId, ca.AccountNumber }).IsUnique();
            e.HasIndex(ca => ca.OrganizationId);
            
            e.Property(ca => ca.CreditLimit).HasColumnType("numeric(19,4)");
            e.Property(ca => ca.AvailableCredit).HasColumnType("numeric(19,4)");
            e.Property(ca => ca.CurrentBalance).HasColumnType("numeric(19,4)");
            e.Property(ca => ca.Version).IsRowVersion();
            
            e.HasMany(ca => ca.Invoices)
             .WithOne(i => i.CustomerAccount)
             .HasForeignKey(i => i.CustomerAccountId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Invoice>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
            e.HasIndex(i => new { i.SourceSystem, i.SourceReferenceId });
            e.HasIndex(i => i.Status);
            
            e.Property(i => i.SubtotalAmount).HasColumnType("numeric(19,4)");
            e.Property(i => i.TaxAmount).HasColumnType("numeric(19,4)");
            e.Property(i => i.TotalAmount).HasColumnType("numeric(19,4)");
            e.Property(i => i.AmountPaid).HasColumnType("numeric(19,4)");
            e.Property(i => i.AmountDue).HasColumnType("numeric(19,4)");
            e.Property(i => i.Version).IsRowVersion();
            
            e.HasMany(i => i.LineItems)
             .WithOne(li => li.Invoice)
             .HasForeignKey(li => li.InvoiceId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<InvoiceLineItem>(e =>
        {
            e.Property(li => li.Quantity).HasColumnType("numeric(19,4)");
            e.Property(li => li.UnitPrice).HasColumnType("numeric(19,4)");
            e.Property(li => li.TaxRate).HasColumnType("numeric(19,4)");
            e.Property(li => li.TaxAmount).HasColumnType("numeric(19,4)");
            e.Property(li => li.TotalAmount).HasColumnType("numeric(19,4)");
        });

        builder.Entity<Receipt>(e =>
        {
            e.HasIndex(r => new { r.TenantId, r.ReceiptNumber }).IsUnique();
            e.HasIndex(r => r.PaymentTransactionId);
            
            e.Property(r => r.Amount).HasColumnType("numeric(19,4)");
            e.Property(r => r.MetadataJson).HasColumnType("jsonb");
            
            e.HasOne(r => r.Invoice)
             .WithMany()
             .HasForeignKey(r => r.InvoiceId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuration
        builder.Entity<PaymentGatewayConfiguration>(e =>
        {
            e.HasIndex(pg => new { pg.TenantId, pg.GatewayName });
            
            e.Property(pg => pg.SupportedCurrenciesJson).HasColumnType("jsonb");
            e.Property(pg => pg.AdditionalSettingsJson).HasColumnType("jsonb");
            e.Property(pg => pg.Version).IsRowVersion();
            
            e.HasMany(pg => pg.SupportedMethods)
             .WithOne(pm => pm.Gateway)
             .HasForeignKey(pm => pm.PaymentGatewayConfigurationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PaymentMethodConfiguration>(e =>
        {
            e.Property(pm => pm.ProcessingFeePercentage).HasColumnType("numeric(19,4)");
            e.Property(pm => pm.FixedProcessingFee).HasColumnType("numeric(19,4)");
            e.Property(pm => pm.MinimumAmount).HasColumnType("numeric(19,4)");
            e.Property(pm => pm.MaximumAmount).HasColumnType("numeric(19,4)");
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
