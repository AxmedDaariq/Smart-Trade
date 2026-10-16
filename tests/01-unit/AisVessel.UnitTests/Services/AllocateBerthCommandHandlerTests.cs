using System;
using System.Threading;
using System.Threading.Tasks;
using AisVessel.Application.Commands;
using AisVessel.Application.Interfaces;
using AisVessel.Domain.Entities;
using AisVessel.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace AisVessel.UnitTests.Services;

public class AllocateBerthCommandHandlerTests
{
    private readonly Mock<IBerthAllocationRepository> _repository = new();
    private readonly AllocateBerthCommandHandler _handler;

    public AllocateBerthCommandHandlerTests()
    {
        _handler = new AllocateBerthCommandHandler(_repository.Object);
    }

    private static AllocateBerthCommand BuildCommand(Guid? tenantId = null, Guid? portCallId = null, Guid? berthId = null)
    {
        return new AllocateBerthCommand
        {
            TenantId = tenantId ?? Guid.NewGuid(),
            PortCallId = portCallId ?? Guid.NewGuid(),
            BerthId = berthId ?? Guid.NewGuid()
        };
    }

    private void SetupRepository(PortCall? portCall, Berth? berth)
    {
        _repository
            .Setup(r => r.GetPortCallAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(portCall);
        _repository
            .Setup(r => r.GetBerthAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(berth);
    }

    [Fact]
    public async Task Handle_WhenPortCallIsAwaitingBerth_ShouldAllocateAndReturnPortCallId()
    {
        // Arrange
        var command = BuildCommand();
        var portCall = new PortCall
        {
            Id = command.PortCallId,
            TenantId = command.TenantId,
            Status = nameof(PortCallStatus.AwaitingBerth)
        };
        var berth = new Berth { Id = command.BerthId, TenantId = command.TenantId };
        SetupRepository(portCall, berth);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(portCall.Id);
        portCall.Status.Should().Be(nameof(PortCallStatus.BerthAllocated));
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldScopeLookupsToTheCallerTenant()
    {
        // Arrange: a tenant must only ever see its own port calls and berths
        var command = BuildCommand();
        SetupRepository(
            new PortCall { Id = command.PortCallId, TenantId = command.TenantId, Status = nameof(PortCallStatus.AwaitingBerth) },
            new Berth { Id = command.BerthId, TenantId = command.TenantId });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repository.Verify(r => r.GetPortCallAsync(command.PortCallId, command.TenantId, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.GetBerthAsync(command.BerthId, command.TenantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPortCallDoesNotExist_ShouldThrow_AndNotSave()
    {
        // Arrange
        var command = BuildCommand();
        SetupRepository(portCall: null, berth: new Berth { Id = Guid.NewGuid() });

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("PortCall not found.");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBerthDoesNotExist_ShouldThrow_AndNotSave()
    {
        // Arrange
        var command = BuildCommand();
        SetupRepository(
            new PortCall { Id = command.PortCallId, TenantId = command.TenantId, Status = nameof(PortCallStatus.AwaitingBerth) },
            berth: null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Berth not found.");
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(nameof(PortCallStatus.Planned))]
    [InlineData(nameof(PortCallStatus.ArrivedAnchorage))]
    [InlineData(nameof(PortCallStatus.Berthed))]
    [InlineData(nameof(PortCallStatus.Departed))]
    [InlineData(nameof(PortCallStatus.Cancelled))]
    public async Task Handle_WhenPortCallIsNotAwaitingBerth_ShouldThrow_AndNotSave(string currentStatus)
    {
        // Arrange: only port calls actually waiting for a berth may be allocated
        var command = BuildCommand();
        var portCall = new PortCall { Id = command.PortCallId, TenantId = command.TenantId, Status = currentStatus };
        SetupRepository(portCall, new Berth { Id = command.BerthId, TenantId = command.TenantId });

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("PortCall must be AwaitingBerth to allocate a berth.");
        portCall.Status.Should().Be(currentStatus); // status must remain untouched
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldForwardTheCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        CancellationToken receivedToken = CancellationToken.None;
        var command = BuildCommand();
        _repository
            .Setup(r => r.GetPortCallAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, Guid, CancellationToken>((_, _, token) => receivedToken = token)
            .ReturnsAsync(new PortCall { Id = command.PortCallId, TenantId = command.TenantId, Status = nameof(PortCallStatus.AwaitingBerth) });
        _repository
            .Setup(r => r.GetBerthAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Berth { Id = command.BerthId, TenantId = command.TenantId });

        // Act
        await _handler.Handle(command, cts.Token);

        // Assert
        receivedToken.Should().Be(cts.Token);
        _repository.Verify(r => r.SaveChangesAsync(cts.Token), Times.Once);
    }
}
