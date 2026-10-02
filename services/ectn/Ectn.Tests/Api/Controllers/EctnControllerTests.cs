using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using Ectn.Api.Controllers;
using Ectn.Application.Commands;
using Xunit;

namespace Ectn.Tests.Api.Controllers;

public class EctnControllerTests
{
    [Fact]
    public async Task Get_Should_Return_Ok_With_Draft_Status()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var controller = new EctnController(mediatorMock.Object);
        var testId = Guid.NewGuid();

        // Act
        var result = await controller.Get(testId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        
        // Use reflection or dynamic to check anonymous type properties
        var value = result.Value;
        var idProperty = value.GetType().GetProperty("Id")?.GetValue(value);
        var statusProperty = value.GetType().GetProperty("Status")?.GetValue(value);
        
        Assert.Equal(testId, idProperty);
        Assert.Equal("Draft", statusProperty);
    }

    [Fact]
    public async Task Create_Should_Return_Forbid_If_TenantId_Is_Missing()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var controller = new EctnController(mediatorMock.Object);
        
        // Mock empty user context (no claims)
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var command = new CreateEctnCommand();

        // Act
        var result = await controller.Create(command) as ForbidResult;

        // Assert
        Assert.NotNull(result); // Should be forbid because tenant_id is missing
    }
}
