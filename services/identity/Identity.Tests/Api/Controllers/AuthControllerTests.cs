using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Api.Controllers;
using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Identity.Tests.Api.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IIdentityAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IIdentityAuthService>();
        
        // Setup HttpContext for Request/Response mocking
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        
        _controller = new AuthController(_mockAuthService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }

    [Fact]
    public async Task Login_Should_Return_Ok_With_Tokens_When_Credentials_Valid()
    {
        // Arrange
        var command = new LoginCommand 
        { 
            TenantCode = "T-001", 
            Email = "admin@example.com", 
            Password = "password123" 
        };

        var expectedResult = new LoginResult
        {
            AccessToken = "mock-access-token",
            RefreshToken = "mock-refresh-token",
            RequiresMfa = false,
            TenantCode = "T-001",
            TenantType = "LogisticsCompany"
        };

        _mockAuthService
            .Setup(s => s.LoginAsync(command, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthController.LoginResponse>(okResult.Value);
        
        Assert.Equal("mock-access-token", response.AccessToken);
        Assert.Equal("LogisticsCompany", response.TenantType);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Credentials_Invalid()
    {
        // Arrange
        var command = new LoginCommand 
        { 
            TenantCode = "T-001", 
            Email = "admin@example.com", 
            Password = "wrongpassword" 
        };

        _mockAuthService
            .Setup(s => s.LoginAsync(command, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResult?)null);

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }
}
