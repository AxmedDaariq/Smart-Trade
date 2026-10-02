using Identity.Application.Commands.Auth;

namespace Identity.Application.Interfaces;

public interface IIdentityAuthService
{
    Task<LoginResult?> LoginAsync(LoginCommand command, string ipAddress, string userAgent, CancellationToken cancellationToken);
    Task<LoginResult?> RefreshAsync(string refreshToken, string ipAddress, string userAgent, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
