using Identity.Application.Commands.Auth;

namespace Identity.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResult?> LoginAsync(LoginCommand command, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
    Task<LoginResult?> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
    Task<bool> RevokeAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);
}
