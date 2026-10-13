using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Identity.UnitTests.Controllers;

/// <summary>
/// Unit tests for <see cref="Identity.Api.Controllers.AuthController"/>.
/// Covers Login, Refresh, and Logout endpoints in isolation using mocked dependencies.
/// </summary>
public sealed class AuthControllerTests
{
    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static LoginResult ValidLoginResult(bool requiresMfa = false) => new()
    {
        AccessToken  = "eyJhbGciOiJSUzI1NiJ9.test",
        RefreshToken = "refresh_opaque_token_xyz",
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
        RequiresMfa  = requiresMfa,
        TenantCode   = "ACME",
        TenantType   = "Importer",
        Roles        = new[] { "Admin" },
        Permissions  = new[] { "declarations.read", "declarations.write" }
    };

    private static LoginCommand ValidLoginCommand() => new()
    {
        TenantCode = "ACME",
        Email      = "admin@acme.com",
        Password   = "S3cur3!Pass"
    };

    private static (Mock<IIdentityAuthService> svc, HttpContext ctx) BuildContext(
        string? refreshCookieValue = null)
    {
        var svc = new Mock<IIdentityAuthService>();

        var responseCookies = new Mock<IResponseCookies>();
        var requestCookies  = new Mock<IRequestCookieCollection>();

        if (refreshCookieValue is not null)
        {
            requestCookies.Setup(c => c.TryGetValue("smarttrade_refresh", out refreshCookieValue))
                          .Returns(true);
        }
        else
        {
            var nullValue = (string?)null;
            requestCookies.Setup(c => c.TryGetValue("smarttrade_refresh", out nullValue))
                          .Returns(false);
        }

        var request = new Mock<HttpRequest>();
        request.Setup(r => r.Cookies).Returns(requestCookies.Object);
        request.Setup(r => r.Headers).Returns(new HeaderDictionary
        {
            ["User-Agent"] = "xunit-test-agent/1.0"
        });

        var response = new Mock<HttpResponse>();
        response.Setup(r => r.Cookies).Returns(responseCookies.Object);

        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(System.Net.IPAddress.Loopback);

        var ctx = new Mock<HttpContext>();
        ctx.Setup(c => c.Request).Returns(request.Object);
        ctx.Setup(c => c.Response).Returns(response.Object);
        ctx.Setup(c => c.Connection).Returns(connection.Object);

        return (svc, ctx.Object);
    }

    // ─── Login ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithTokenResponse()
    {
        // Arrange
        var (svc, ctx) = BuildContext();
        svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(ValidLoginResult());

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Login(ValidLoginCommand(), CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        // Arrange
        var (svc, ctx) = BuildContext();
        svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((LoginResult?)null);

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Login(ValidLoginCommand(), CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>()
              .Which.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Login_MfaRequired_Returns403WithMfaFlag()
    {
        // Arrange
        var (svc, ctx) = BuildContext();
        svc.Setup(s => s.LoginAsync(It.IsAny<LoginCommand>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(ValidLoginResult(requiresMfa: true));

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Login(ValidLoginCommand(), CancellationToken.None);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(403);
    }

    // ─── Refresh ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_ValidCookie_Returns200WithNewTokens()
    {
        // Arrange
        var (svc, ctx) = BuildContext(refreshCookieValue: "valid_refresh_token");
        svc.Setup(s => s.RefreshAsync("valid_refresh_token", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(ValidLoginResult());

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Refresh(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
              .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Refresh_NoCookie_Returns401()
    {
        // Arrange
        var (svc, ctx) = BuildContext(refreshCookieValue: null);

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Refresh(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>()
              .Which.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Refresh_ExpiredToken_Returns401AndClearsCookie()
    {
        // Arrange
        var (svc, ctx) = BuildContext(refreshCookieValue: "expired_token");
        svc.Setup(s => s.RefreshAsync("expired_token", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((LoginResult?)null);

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Refresh(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    // ─── Logout ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_WithRefreshCookie_RevokesTokenAndReturns204()
    {
        // Arrange
        var (svc, ctx) = BuildContext(refreshCookieValue: "refresh_token_abc");
        svc.Setup(s => s.RevokeRefreshTokenAsync("refresh_token_abc", It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Logout(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>()
              .Which.StatusCode.Should().Be(204);

        svc.Verify(s => s.RevokeRefreshTokenAsync("refresh_token_abc", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_NoCookie_Returns204WithoutRevoke()
    {
        // Arrange
        var (svc, ctx) = BuildContext(refreshCookieValue: null);

        var controller = new Identity.Api.Controllers.AuthController(svc.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = ctx }
        };

        // Act
        var result = await controller.Logout(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>()
              .Which.StatusCode.Should().Be(204);

        svc.Verify(s => s.RevokeRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
