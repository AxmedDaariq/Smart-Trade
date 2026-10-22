using System;
using System.Threading;
using System.Threading.Tasks;
using Ectn.Application.Commands;
using Ectn.Domain.Enums;
using Xunit;

namespace Ectn.Tests.Application.Commands;

public class CreateEctnCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_And_Return_New_Ectn_Id()
    {
        // Arrange
        var handler = new CreateEctnCommandHandler();
        var command = new CreateEctnCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-2026-001",
            Type = EctnType.Standard,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var resultId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);
    }
}
