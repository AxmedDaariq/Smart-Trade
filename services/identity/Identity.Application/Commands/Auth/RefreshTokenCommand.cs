namespace Identity.Application.Commands.Auth;

public sealed class RefreshTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
}
