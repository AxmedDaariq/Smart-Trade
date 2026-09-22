using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Persistence; // Ensure SmartDbContext is accessible

namespace Identity.Infrastructure.Persistence;

public class IdentityDbContext : SmartDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MfaConfiguration> MfaConfigurations => Set<MfaConfiguration>();
    public DbSet<MfaRecoveryCode> MfaRecoveryCodes => Set<MfaRecoveryCode>();
    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<SecurityEvent> SecurityEvents => Set<SecurityEvent>();
    public DbSet<IdentityAuditEntry> IdentityAuditEntries => Set<IdentityAuditEntry>();
    public DbSet<UserSecurityState> UserSecurityStates => Set<UserSecurityState>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Tenant
        builder.Entity<Tenant>(e => 
        {
            e.HasIndex(t => t.TenantCode).IsUnique();
            e.HasIndex(t => t.Status);
            e.HasIndex(t => t.TenantType);
            e.Property(t => t.Version).IsRowVersion();
        });

        // Organization
        builder.Entity<Organization>(e => 
        {
            e.HasIndex(o => new { o.TenantId, o.OrganizationCode }).IsUnique();
            e.HasIndex(o => o.TenantId);
            e.HasIndex(o => new { o.TenantId, o.Status });
            e.HasIndex(o => o.ParentOrganizationId);
            e.Property(o => o.Version).IsRowVersion();
            
            // Self-referencing FK for Parent
            e.HasOne<Organization>().WithMany().HasForeignKey(o => o.ParentOrganizationId).OnDelete(DeleteBehavior.Restrict);
        });

        // User
        builder.Entity<User>(e => 
        {
            e.HasIndex(u => new { u.TenantId, u.NormalizedUsername }).IsUnique();
            e.HasIndex(u => u.TenantId);
            e.HasIndex(u => u.OrganizationId);
            e.HasIndex(u => u.Status);
            e.HasIndex(u => u.NormalizedEmail);
            e.Property(u => u.Version).IsRowVersion();
            
            e.HasOne<Tenant>().WithMany().HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Organization>().WithMany().HasForeignKey(u => u.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        });

        // Role
        builder.Entity<Role>(e => 
        {
            e.HasIndex(r => new { r.NormalizedName, r.TenantId }).IsUnique();
            e.HasIndex(r => r.TenantId);
            e.Property(r => r.Version).IsRowVersion();
        });

        // Permission
        builder.Entity<Permission>(e => 
        {
            e.HasIndex(p => p.Code).IsUnique();
        });

        // UserRole
        builder.Entity<UserRole>(e => 
        {
            e.HasKey(ur => new { ur.UserId, ur.RoleId });
            e.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission
        builder.Entity<RolePermission>(e => 
        {
            e.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            e.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
        });

        // Session
        builder.Entity<Session>(e => 
        {
            e.HasIndex(s => s.SessionReference).IsUnique();
            e.HasOne<User>().WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.Version).IsRowVersion();
        });

        // RefreshToken
        builder.Entity<RefreshToken>(e => 
        {
            e.HasIndex(rt => rt.TokenHash);
            e.HasIndex(rt => rt.TokenFamilyId);
            e.HasIndex(rt => rt.UserId);
            e.HasIndex(rt => rt.SessionId);
            e.HasIndex(rt => rt.ExpiresAtUtc);
            
            e.HasOne(rt => rt.Session).WithMany(s => s.RefreshTokens).HasForeignKey(rt => rt.SessionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<User>().WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // MfaConfiguration
        builder.Entity<MfaConfiguration>(e => 
        {
            e.HasOne<User>().WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(m => m.Version).IsRowVersion();
        });

        // MfaRecoveryCode
        builder.Entity<MfaRecoveryCode>(e => 
        {
            e.HasIndex(m => m.MfaConfigurationId);
            e.HasIndex(m => m.Status);
            e.HasOne(m => m.MfaConfiguration).WithMany(mc => mc.RecoveryCodes).HasForeignKey(m => m.MfaConfigurationId).OnDelete(DeleteBehavior.Cascade);
        });

        // PasswordHistory
        builder.Entity<PasswordHistory>(e => 
        {
            e.HasIndex(p => new { p.UserId, p.CreatedAtUtc });
            e.HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // EmailVerificationToken
        builder.Entity<EmailVerificationToken>(e => 
        {
            e.HasIndex(evt => evt.UserId);
            e.HasIndex(evt => evt.TokenHash);
            e.HasIndex(evt => evt.ExpiresAtUtc);
            e.HasOne<User>().WithMany().HasForeignKey(evt => evt.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // PasswordResetToken
        builder.Entity<PasswordResetToken>(e => 
        {
            e.HasIndex(prt => prt.UserId);
            e.HasIndex(prt => prt.TokenHash);
            e.HasOne<User>().WithMany().HasForeignKey(prt => prt.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // SecurityEvent
        builder.Entity<SecurityEvent>(e => 
        {
            e.HasIndex(se => new { se.TenantId, se.OccurredAtUtc });
            e.HasIndex(se => new { se.UserId, se.OccurredAtUtc });
            e.HasIndex(se => se.EventType);
            e.HasIndex(se => se.Severity);
            
            e.Property(se => se.MetadataJson).HasColumnType("jsonb");
        });

        // IdentityAuditEntry
        builder.Entity<IdentityAuditEntry>(e => 
        {
            e.HasIndex(ia => new { ia.TenantId, ia.OccurredAtUtc });
            e.HasIndex(ia => ia.ActorUserId);
            e.HasIndex(ia => new { ia.ResourceType, ia.ResourceId });
            
            e.Property(ia => ia.BeforeJson).HasColumnType("jsonb");
            e.Property(ia => ia.AfterJson).HasColumnType("jsonb");
        });

        // UserSecurityState
        builder.Entity<UserSecurityState>(e => 
        {
            e.HasOne(uss => uss.User).WithMany().HasForeignKey(uss => uss.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(uss => uss.Version).IsRowVersion();
        });
    }
}
