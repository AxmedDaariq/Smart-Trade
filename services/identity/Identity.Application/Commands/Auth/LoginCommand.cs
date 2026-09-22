namespace Identity.Application.Commands.Auth;

public class LoginCommand
{
    public string TenantCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    
    public bool RequiresMfa { get; set; }
}
