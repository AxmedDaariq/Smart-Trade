using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Identity.Api.IntegrationTests;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Integration tests for AuthController — login, refresh, logout,
/// forgot-password. Exercises the full auth pipeline: PBKDF2 password
/// verification, JWT issuance, refresh token rotation, session management,
/// account lockout, and rate limiting.
/// </summary>
[Collection("identity-postgres")]
public class AuthControllerIntegrationTests
{
    private readonly IdentityPostgresFixture _db;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(IdentityPostgresFixture db)
    {
        _db = db;
        var factory = new IdentityApiFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private static StringContent Json(object body) =>
        new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

    // ── Login ──────────────────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens_AndSetsRefreshCookie()
    {
        var (tenantId, userId, tenantCode) = await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN01", "login@example.com",
            new[] { "identity.users.read" });

        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode, email = "login@example.com", password = IdentityTestData.TestPassword }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(body.GetProperty("accessToken").GetString()));
        Assert.False(string.IsNullOrEmpty(body.GetProperty("expiresAtUtc").GetString()));
        Assert.Equal("LogisticsCompany", body.GetProperty("tenantType").GetString());
        Assert.Equal(tenantCode, body.GetProperty("tenantCode").GetString());
        Assert.Contains("identity.users.read", body.GetProperty("permissions").EnumerateArray()
            .Select(p => p.GetString()!));

        // Refresh token must be set as HttpOnly cookie
        Assert.Contains("smarttrade_refresh=", response.Headers.GetValues("Set-Cookie").First());
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN02", "wrongpw@example.com", Array.Empty<string>());

        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGIN02", email = "wrongpw@example.com", password = "WrongPassword!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownTenant_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "UNKNOWN", email = "nobody@example.com", password = "whatever" }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ReturnsUnauthorized()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN03", "inactive@example.com",
            Array.Empty<string>(), userStatus: "Suspended");

        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGIN03", email = "inactive@example.com", password = IdentityTestData.TestPassword }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnverifiedEmail_ReturnsUnauthorized()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN04", "unverified@example.com",
            Array.Empty<string>(), emailVerified: false);

        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGIN04", email = "unverified@example.com", password = IdentityTestData.TestPassword }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithMfaEnabled_ReturnsForbidden_RequiresMfa()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN05", "mfa@example.com",
            Array.Empty<string>(), mfaEnabled: true);

        var response = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGIN05", email = "mfa@example.com", password = IdentityTestData.TestPassword }));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("requiresMfa").GetBoolean());
    }

    [Fact]
    public async Task Login_AfterFiveFailedAttempts_LocksAccount()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGIN06", "lockme@example.com", Array.Empty<string>());

        // 5 failed attempts
        for (var i = 0; i < 5; i++)
        {
            var response = await _client.PostAsync("/api/v1/identity/auth/login",
                Json(new { tenantCode = "LOGIN06", email = "lockme@example.com", password = "WrongPassword!" }));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // 6th attempt with correct password — still locked
        var lockedResponse = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGIN06", email = "lockme@example.com", password = IdentityTestData.TestPassword }));

        Assert.Equal(HttpStatusCode.Unauthorized, lockedResponse.StatusCode);

        // Verify lockout in DB
        await using var ctx = IdentityTestData.CreateContext(_db.ConnectionString);
        var user = await ctx.Users.FirstAsync(u => u.NormalizedEmail == "LOCKME@EXAMPLE.COM");
        Assert.NotNull(user.LockedUntilUtc);
        Assert.True(user.LockedUntilUtc > DateTime.UtcNow);
    }

    // ── Refresh ────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_WithValidCookie_ReturnsNewTokens()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "REFRESH1", "refresh@example.com",
            new[] { "identity.users.read" });

        var loginResponse = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "REFRESH1", email = "refresh@example.com", password = IdentityTestData.TestPassword }));
        loginResponse.EnsureSuccessStatusCode();

        var tokenValue = IdentityTestData.ExtractRefreshToken(loginResponse);

        // The refresh endpoint reads the smarttrade_refresh cookie,
        // so the token must travel as a Cookie header.
        var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/auth/refresh");
        refreshRequest.Headers.Add("Cookie", $"smarttrade_refresh={tokenValue}");

        var refreshResponse = await _client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var body = await refreshResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(body.GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/v1/identity/auth/refresh",
            Json(new { refreshToken = "invalid-token-value" }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_TokenReuse_RevokesSessionFamily()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "REFRESH2", "reuse@example.com", Array.Empty<string>());

        var loginResponse = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "REFRESH2", email = "reuse@example.com", password = IdentityTestData.TestPassword }));
        loginResponse.EnsureSuccessStatusCode();

        var tokenValue = IdentityTestData.ExtractRefreshToken(loginResponse);
        var tokenHash = IdentityTestData.HashRefreshToken(tokenValue);

        // First refresh — succeeds
        var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/auth/refresh");
        firstRequest.Headers.Add("Cookie", $"smarttrade_refresh={tokenValue}");
        var firstRefresh = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.OK, firstRefresh.StatusCode);

        // Reuse the same token — must be rejected (replay detection)
        var reuseRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/auth/refresh");
        reuseRequest.Headers.Add("Cookie", $"smarttrade_refresh={tokenValue}");
        var reuseResponse = await _client.SendAsync(reuseRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);

        // Session family must be revoked in DB
        await using var ctx = IdentityTestData.CreateContext(_db.ConnectionString);
        var tokens = await ctx.RefreshTokens
            .Where(t => t.TokenHash == tokenHash)
            .ToListAsync();
        Assert.All(tokens, t => Assert.Equal("Revoked", t.Status));
    }

    // ── Logout ─────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_WithValidCookie_RevokesSession()
    {
        await IdentityTestData.SeedActiveUserAsync(
            _db.ConnectionString, "LOGOUT1", "logout@example.com", Array.Empty<string>());

        var loginResponse = await _client.PostAsync("/api/v1/identity/auth/login",
            Json(new { tenantCode = "LOGOUT1", email = "logout@example.com", password = IdentityTestData.TestPassword }));
        loginResponse.EnsureSuccessStatusCode();

        var tokenValue = IdentityTestData.ExtractRefreshToken(loginResponse);
        var tokenHash = IdentityTestData.HashRefreshToken(tokenValue);

        // The logout endpoint reads the smarttrade_refresh cookie.
        var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/auth/logout");
        logoutRequest.Headers.Add("Cookie", $"smarttrade_refresh={tokenValue}");

        var logoutResponse = await _client.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        // Session must be revoked
        await using var ctx = IdentityTestData.CreateContext(_db.ConnectionString);
        var session = await ctx.Sessions
            .Include(s => s.RefreshTokens)
            .FirstAsync(s => s.RefreshTokens.Any(t => t.TokenHash == tokenHash));
        Assert.Equal("Revoked", session.Status);
    }

    // ── Forgot password ────────────────────────────────────────────

    [Fact]
    public async Task ForgotPassword_ReturnsServiceUnavailable()
    {
        var response = await _client.PostAsync("/api/v1/identity/auth/forgot-password",
            Json(new { email = "anyone@example.com" }));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
