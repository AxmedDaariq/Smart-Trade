using System;
using System.Linq;
using System.Threading.Tasks;
using AisVessel.Domain.Entities;
using AisVessel.Domain.Enums;
using AisVessel.Infrastructure.Persistence;
using AisVessel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AisVessel.Tests.Infrastructure.Repositories;

public class BerthAllocationRepositoryTests : IDisposable
{
    private readonly AisDbContext _context;
    private readonly BerthAllocationRepository _repository;

    public BerthAllocationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AisDbContext>()
            .UseInMemoryDatabase($"ais-berthallocation-tests-{Guid.NewGuid()}")
            .Options;

        _context = new AisDbContext(options);
        _repository = new BerthAllocationRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    private async Task<PortCall> SeedPortCallAsync(Guid tenantId, PortCallStatus status = PortCallStatus.AwaitingBerth)
    {
        var portCall = new PortCall
        {
            TenantId = tenantId,
            PortId = Guid.NewGuid(),
            VesselId = Guid.NewGuid(),
            PortCallReference = $"PC-{Guid.NewGuid():N}",
            Status = status.ToString()
        };

        _context.PortCalls.Add(portCall);
        await _context.SaveChangesAsync();
        return portCall;
    }

    private async Task<Berth> SeedBerthAsync(Guid tenantId)
    {
        var berth = new Berth
        {
            TenantId = tenantId,
            PortId = Guid.NewGuid(),
            BerthReference = $"B-{Guid.NewGuid():N}",
            Name = "Berth 1"
        };

        _context.Berths.Add(berth);
        await _context.SaveChangesAsync();
        return berth;
    }

    [Fact]
    public async Task GetPortCallAsync_WhenPortCallBelongsToTheTenant_ShouldReturnIt()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var portCall = await SeedPortCallAsync(tenantId);

        // Act
        var result = await _repository.GetPortCallAsync(portCall.Id, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(portCall.Id, result!.Id);
        Assert.Equal(tenantId, result.TenantId);
    }

    [Fact]
    public async Task GetPortCallAsync_WhenPortCallBelongsToAnotherTenant_ShouldReturnNull()
    {
        // Arrange: tenant isolation must be enforced by the query itself
        var portCall = await SeedPortCallAsync(Guid.NewGuid());

        // Act
        var result = await _repository.GetPortCallAsync(portCall.Id, Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPortCallAsync_WhenPortCallDoesNotExist_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _repository.GetPortCallAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBerthAsync_WhenBerthBelongsToTheTenant_ShouldReturnIt()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var berth = await SeedBerthAsync(tenantId);

        // Act
        var result = await _repository.GetBerthAsync(berth.Id, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(berth.Id, result!.Id);
        Assert.Equal(tenantId, result.TenantId);
    }

    [Fact]
    public async Task GetBerthAsync_WhenBerthBelongsToAnotherTenant_ShouldReturnNull()
    {
        // Arrange
        var berth = await SeedBerthAsync(Guid.NewGuid());

        // Act
        var result = await _repository.GetBerthAsync(berth.Id, Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistStatusChangesMadeToAPortCall()
    {
        // Arrange: mirrors the AllocateBerthCommandHandler flow
        var tenantId = Guid.NewGuid();
        var portCall = await SeedPortCallAsync(tenantId);
        var loaded = await _repository.GetPortCallAsync(portCall.Id, tenantId);

        // Act
        loaded!.AdvanceState(PortCallStatus.BerthAllocated);
        await _repository.SaveChangesAsync();

        // Assert: clear the tracker so the assertion reads from the store, not the cache
        _context.ChangeTracker.Clear();
        var persisted = await _context.PortCalls.AsNoTracking().SingleOrDefaultAsync(p => p.Id == portCall.Id);

        Assert.NotNull(persisted);
        Assert.Equal(nameof(PortCallStatus.BerthAllocated), persisted!.Status);
        Assert.Equal(tenantId, persisted.TenantId);
    }
}