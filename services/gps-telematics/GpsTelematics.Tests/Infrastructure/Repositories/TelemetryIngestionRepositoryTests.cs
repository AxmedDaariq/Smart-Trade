using System;
using System.Linq;
using System.Threading.Tasks;
using GpsTelematics.Domain.Entities;
using GpsTelematics.Infrastructure.Persistence;
using GpsTelematics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GpsTelematics.Tests.Infrastructure.Repositories;

public class TelemetryIngestionRepositoryTests : IDisposable
{
    private readonly GpsDbContext _context;
    private readonly TelemetryIngestionRepository _repository;

    public TelemetryIngestionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<GpsDbContext>()
            .UseInMemoryDatabase($"gps-telemetry-tests-{Guid.NewGuid()}")
            .Options;

        _context = new GpsDbContext(options);
        _repository = new TelemetryIngestionRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    private static GpsPosition BuildPosition(
        double latitude = 11.5919,
        double longitude = 43.1456,
        long sequenceNumber = 1001)
    {
        return new GpsPosition
        {
            TenantId = Guid.NewGuid(),
            GpsDeviceId = Guid.NewGuid(),
            VehicleReference = "VEH-42",
            Position = new Point(longitude, latitude),
            Latitude = latitude,
            Longitude = longitude,
            SpeedKph = 42.5,
            HeadingDegrees = 187.3,
            TimestampUtc = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            SequenceNumber = sequenceNumber
        };
    }

    [Fact]
    public async Task AddAsync_ShouldPersistThePositionWithoutAnExplicitSaveCall()
    {
        // Arrange: the repository is responsible for calling SaveChanges itself
        var position = BuildPosition();

        // Act
        await _repository.AddAsync(position);

        // Assert: a fresh query hits the store, not the change tracker
        var persisted = await _context.GpsPositions
            .AsNoTracking()
            .SingleAsync(p => p.Id == position.Id);

        Assert.NotNull(persisted);
        Assert.Equal(position.TenantId, persisted.TenantId);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistAllIngestedFields()
    {
        // Arrange
        var position = BuildPosition(latitude: 11.5919, longitude: 43.1456, sequenceNumber: 777);

        // Act
        await _repository.AddAsync(position);

        // Assert
        var persisted = await _context.GpsPositions.AsNoTracking().SingleAsync(p => p.Id == position.Id);
        Assert.Equal(11.5919, persisted.Latitude);
        Assert.Equal(43.1456, persisted.Longitude);
        Assert.Equal(42.5, persisted.SpeedKph);
        Assert.Equal(187.3, persisted.HeadingDegrees);
        Assert.Equal(777, persisted.SequenceNumber);
        Assert.Equal("VEH-42", persisted.VehicleReference);
        Assert.Equal("Accepted", persisted.ValidationStatus);
        Assert.Equal(new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc), persisted.TimestampUtc);
    }

    [Fact]
    public async Task AddAsync_CalledTwice_ShouldPersistBothPositions()
    {
        // Arrange: a device sends a stream of frames across multiple calls
        var first = BuildPosition(sequenceNumber: 1);
        var second = BuildPosition(sequenceNumber: 2);

        // Act
        await _repository.AddAsync(first);
        await _repository.AddAsync(second);

        // Assert
        Assert.Equal(2, await _context.GpsPositions.AsNoTracking().CountAsync());
    }

    [Fact]
    public async Task AddAsync_WithAnAlreadyStoredId_ShouldThrow_RatherThanDuplicateTheRow()
    {
        // Arrange: the primary key prevents a second row for the same frame
        var position = BuildPosition();
        await _repository.AddAsync(position);
        _context.ChangeTracker.Clear(); // simulate a brand-new unit of work

        // Act & Assert: re-ingestion of the same Id is rejected by the store
        await Assert.ThrowsAnyAsync<Exception>(() => _repository.AddAsync(position));
        Assert.Equal(1, await _context.GpsPositions.AsNoTracking().CountAsync());
    }
}