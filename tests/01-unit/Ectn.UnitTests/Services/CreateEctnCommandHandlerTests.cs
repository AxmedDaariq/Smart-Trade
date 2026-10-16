using System;
using System.Threading;
using System.Threading.Tasks;
using Ectn.Application.Commands;
using Ectn.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ectn.UnitTests.Services;

public class CreateEctnCommandHandlerTests
{
    private readonly CreateEctnCommandHandler _handler;

    public CreateEctnCommandHandlerTests()
    {
        _handler = new CreateEctnCommandHandler();
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnNewEctnId()
    {
        // Arrange
        var command = new CreateEctnCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-2026-001",
            Type = EctnType.Standard,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenTenantIdIsEmpty()
    {
        // Arrange
        var command = new CreateEctnCommand
        {
            TenantId = Guid.Empty,
            ApplicationReference = "APP-2026-001",
            Type = EctnType.Standard,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*TenantId cannot be empty*");
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenApplicationReferenceIsEmpty()
    {
        // Arrange
        var command = new CreateEctnCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "",
            Type = EctnType.Standard,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ApplicationReference cannot be empty*");
    }
}
