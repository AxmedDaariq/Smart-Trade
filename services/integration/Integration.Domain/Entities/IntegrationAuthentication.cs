namespace Integration.Domain.Entities;

public class IntegrationAuthentication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    
    public string AuthenticationReference { get; set; } = string.Empty;
    
    public string AuthenticationType { get; set; } = string.Empty; // OAuth2ClientCredentials, ApiKeyReference, BasicAuthReference, HmacSignature, MutualTLS, SignedRequest, JwtClientAssertion, None, Other
    
    public string VaultSecretPath { get; set; } = string.Empty;
    public string? CertificateReference { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
}
