using Moq;
using FluentAssertions;
using Corridor.Application.Commands;
using Corridor.Domain.Entities;
using Xunit;
using MockQueryable.Moq;

namespace Corridor.UnitTests.Application;

public class DetectRouteDeviationCommandHandlerTests
{
    // NOTE: Handler tests that exercise PostGIS geometry (Distance) require
    // a real RouteVersion with Geometry populated — those are integration tests.
    // Here we focus on the guard clauses that are pure C# and have no GIS dependency.

    private readonly Mock<ICorridorDbContext> _dbMock;
    private readonly DetectRouteDeviationCommandHandler _handler;

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid TripId   = Guid.NewGuid();

    public DetectRouteDeviationCommandHandlerTests()
    {
        _dbMock  = new Mock<ICorridorDbContext>();
        _handler = new DetectRouteDeviationCommandHandler(_dbMock.Object);
    }

    // ── Guard: Trip not found ──────────────────────────────────────────────────

    [Fact]
    public async Task Handle_TripNotFound_ThrowsInvalidOperationException()
    {
        var emptyTrips = Enumerable.Empty<Trip>().AsQueryable().BuildMock();
        _dbMock.Setup(x => x.Trips).Returns(emptyTrips.Object);

        var command = BuildCommand();

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                  .WithMessage("*Trip not found*");
    }

    // ── Guard: RouteVersion not found ─────────────────────────────────────────

    [Fact]
    public async Task Handle_RouteVersionNotFound_ThrowsInvalidOperationException()
    {
        var trip = new Trip
        {
            Id             = TripId,
            TenantId       = TenantId,
            RouteVersionId = Guid.NewGuid() // points to a version that won't exist
        };

        var trips         = new List<Trip> { trip }.AsQueryable().BuildMock();
        var emptyVersions = Enumerable.Empty<RouteVersion>().AsQueryable().BuildMock();

        _dbMock.Setup(x => x.Trips).Returns(trips.Object);
        _dbMock.Setup(x => x.RouteVersions).Returns(emptyVersions.Object);

        var command = BuildCommand(tripId: TripId);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
                  .WithMessage("*RouteVersion not found*");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static DetectRouteDeviationCommand BuildCommand(
        Guid? tripId    = null,
        double latitude  = 2.0469,
        double longitude = 45.3182) => new()
    {
        TenantId  = TenantId,
        TripId    = tripId ?? TripId,
        Latitude  = latitude,
        Longitude = longitude
    };
}