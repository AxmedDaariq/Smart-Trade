using Identity.Api.Controllers;
using Identity.Application.Commands.Auth;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Xunit;

namespace Identity.UnitTests.Controllers;

/// <summary>
/// Direct Integration Test:
/// Xidhiidh toos ah oo ka dhexeeya AuthController dhabta ah iyo IdentityAuthService dhabta ah
/// iyada oo aan la isticmaalin mock services.
/// </summary>
public sealed class AuthControllerLiveServiceTests
{
    private readonly IdentityDbContext _dbContext;
    private readonly IdentityAuthService _realAuthService;
    private readonly AuthController _controller;

    public AuthControllerLiveServiceTests()
    {
        // 1. Setup in-memory Real Database
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: $"IdentityTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new IdentityDbContext(options);

        // 2. Setup Real PasswordHasher iyo Real JwtGenerator
        var passwordHasher = new Pbkdf2PasswordHasher();
        var jwtOptions = Options.Create(new JwtTokenOptions
        {
            Secret = "0123456789abcdef0123456789abcdef",
            Issuer = "smarttrade-test",
            Audience = "smarttrade-test",
            AccessTokenLifetimeMinutes = 15
        });
        var jwtTokenGenerator = new JwtTokenGenerator(jwtOptions);

        // 3. Create REAL IdentityAuthService (Service-ka rasmiga ah ee mashruuca)
        _realAuthService = new IdentityAuthService(
            _dbContext,
            passwordHasher,
            jwtTokenGenerator,
            jwtOptions,
            NullLogger<IdentityAuthService>.Instance
        );

        // 4. Inject REAL service into the REAL AuthController
        _controller = new AuthController(_realAuthService);

        // Setup real HttpContext for controller
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers["User-Agent"] = "LiveIntegrationTest/1.0";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_WithRealControllerAndRealService_ShouldSucceed_WhenUserExistsInDatabase()
    {
        // Arrange: Ku kaydi Tenant iyo User dhab ah Database-ka
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            TenantCode = "SOM-LOGISTICS",
            Name = "Somalia Logistics Co",
            Status = "Active"
        };

        var hasher = new Pbkdf2PasswordHasher();
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = "admin@somlogistics.com",
            NormalizedEmail = "ADMIN@SOMLOGISTICS.COM",
            Username = "admin",
            NormalizedUsername = "ADMIN",
            PasswordHash = hasher.HashPassword("P@ssword123!"),
            EmailVerified = true,
            Status = "Active"
        };

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var loginCommand = new LoginCommand
        {
            TenantCode = "SOM-LOGISTICS",
            Email = "admin@somlogistics.com",
            Password = "P@ssword123!"
        };

        // Act: Controller-ka toos ayaan u wacaynaa -> wuxuuna wacayaa Real Service -> DB & Password Hasher dhab ah
        var actionResult = await _controller.Login(loginCommand, CancellationToken.None);

        // Assert: Hubi in Controller-ku soo celiyey 200 OK oo wata JWT Token dhab ah
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsType<AuthController.LoginResponse>(okResult.Value);

        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.Equal("SOM-LOGISTICS", response.TenantCode);
        Assert.True(response.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithRealControllerAndRealService_ShouldFail_WhenPasswordIsWrong()
    {
        // Arrange: Ku kaydi Tenant iyo User dhab ah
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            TenantCode = "GLOBAL-TRADERS",
            Name = "Global Traders Inc",
            Status = "Active"
        };

        var hasher = new Pbkdf2PasswordHasher();
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = "manager@global.com",
            NormalizedEmail = "MANAGER@GLOBAL.COM",
            PasswordHash = hasher.HashPassword("CorrectPassword123!"),
            Status = "Active"
        };

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var wrongPasswordCommand = new LoginCommand
        {
            TenantCode = "GLOBAL-TRADERS",
            Email = "manager@global.com",
            Password = "WrongPassword999!"
        };

        // Act: Baaq toos ah oo ku dhacaya Controller iyo Real Service
        var actionResult = await _controller.Login(wrongPasswordCommand, CancellationToken.None);

        // Assert: Waa inuu soo celiyaa 401 Unauthorized
        Assert.IsType<UnauthorizedObjectResult>(actionResult);
    }
}
