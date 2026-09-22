namespace Notification.Domain.Entities;

public class NotificationRecipient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public string UserReference { get; set; } = string.Empty;
    public string? OrganizationReference { get; set; }
    
    public string RecipientType { get; set; } = "User"; // User, Organization, Role, Group, ExternalAddress
    public string? AddressReference { get; set; }
    
    public string Status { get; set; } = "Pending";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }

    public Notification Notification { get; set; } = null!;
}
