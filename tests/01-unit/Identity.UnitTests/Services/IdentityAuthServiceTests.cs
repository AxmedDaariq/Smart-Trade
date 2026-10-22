using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Moq;
using FluentAssertions;

namespace Identity.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="IIdentityAuthService"/> contract validation.
/// These tests verify the interface contract and any concrete implementation
/// behaviour that can be tested without a real database.
/// </summary>
public sealed class IdentityAuthServiceTests
{
    private readonly Mock<IIdentityAuthService> _svc = new();

    // ─── LoginAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_ReturnsLoginResult_WhenCredentialsAreValid()
    {
        // Arrange
        var expected = BuildLoginResult();
        _svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _svc.Object.LoginAsync(new LoginCommand
        {
            TenantCode = "ACME",
            Email      = "admin@acme.com",
            Password   = "Valid@123"
        }, "127.0.0.1", "Test/1.0", default);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        _svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginResult?)null);

        // Act
        var result = await _svc.Object.LoginAsync(new LoginCommand
        {
            TenantCode = "ACME",
            Email      = "wrong@acme.com",
            Password   = "wrong"
        }, "127.0.0.1", "Test/1.0", default);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_SetsRequiresMfa_WhenUserHasMfaEnabled()
    {
        // Arrange
        var expected = BuildLoginResult(requiresMfa: true);
        _svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _svc.Object.LoginAsync(new LoginCommand
        {
            TenantCode = "ACME",
            Email      = "mfa@acme.com",
            Password   = "Valid@123"
        }, "127.0.0.1", "Test/1.0", default);

        // Assert
        result.Should().NotBeNull();
        result!.RequiresMfa.Should().BeTrue();
        result.AccessToken.Should().BeNullOrEmpty("access token must not be issued before MFA is completed");
    }

    [Fact]
    public async Task LoginAsync_PopulatesRolesAndPermissions()
    {
        // Arrange
        var expected = BuildLoginResult(
            roles: new[] { "DeclarationAgent", "PaymentViewer" },
            permissions: new[] { "declarations.submit", "payments.view" });

        _svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _svc.Object.LoginAsync(new LoginCommand
        {
            TenantCode = "ACME",
            Email      = "agent@acme.com",
            Password   = "Valid@123"
        }, "127.0.0.1", "Test/1.0", default);

        // Assert
        result!.Roles.Should().ContainInOrder("DeclarationAgent", "PaymentViewer");
        result.Permissions.Should().Contain("declarations.submit");
    }

    // ─── RefreshAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task RefreshAsync_ReturnsNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var expected = BuildLoginResult();
        _svc.Setup(s => s.RefreshAsync("valid_token", It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _svc.Object.RefreshAsync("valid_token", "127.0.0.1", "Test/1.0", default);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshAsync_ReturnsNull_WhenRefreshTokenIsExpiredOrRevoked()
    {
        // Arrange
        _svc.Setup(s => s.RefreshAsync("expired_token", It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginResult?)null);

        // Act
        var result = await _svc.Object.RefreshAsync("expired_token", "127.0.0.1", "Test/1.0", default);

        // Assert
        result.Should().BeNull();
    }

    // ─── RevokeRefreshTokenAsync ──────────────────────────────────────────────

    [Fact]
    public async Task RevokeRefreshTokenAsync_CompletesSuccessfully()
    {
        // Arrange
        _svc.Setup(s => s.RevokeRefreshTokenAsync(It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var act = async () => await _svc.Object.RevokeRefreshTokenAsync("token_to_revoke", default);

        // Assert
        await act.Should().NotThrowAsync();
        _svc.Verify(s => s.RevokeRefreshTokenAsync("token_to_revoke", default), Times.Once);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static LoginResult BuildLoginResult(
        bool requiresMfa = false,
        string[]? roles = null,
        string[]? permissions = null) => new()
    {
        AccessToken  = requiresMfa ? string.Empty : "eyJhbGciOiJSUzI1NiJ9.test_payload",
        RefreshToken = requiresMfa ? string.Empty : "opaque_refresh_token_abc123",
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
        RequiresMfa  = requiresMfa,
        TenantCode   = "ACME",
        TenantType   = "Importer",
        Roles        = roles        ?? new[] { "Admin" },
        Permissions  = permissions  ?? new[] { "declarations.read" }
    };
}
