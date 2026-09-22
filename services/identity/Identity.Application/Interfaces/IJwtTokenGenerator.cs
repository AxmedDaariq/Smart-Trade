using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, Tenant tenant, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    string HashToken(string token);
}
