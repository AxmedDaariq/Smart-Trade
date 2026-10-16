using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Corridor.Application.Commands;
using Corridor.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Corridor.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="DetectRouteDeviationCommandHandler"/>. 
/// Validates route-deviation detection logic using mocked <see cref="ICorridorDbContext"/>.
/// </summary>
public sealed class DetectRouteDeviationCommandHandlerTests
{
    private readonly Mock<ICorridorDbContext> _contextMock = new();

    // ─── Trip not found ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_TripNotFound_ThrowsInvalidOperationException()
    {
        // Arrange – empty Trips queryable
        _contextMock
            .Setup(c => c.Trips)
            .Returns(Array.Empty<Trip>().AsQueryable().BuildMockDbSet().Object);

        var handler = new DetectRouteDeviationCommandHandler(_contextMock.Object);
        var command = new DetectRouteDeviationCommand
        {
            TenantId = Guid.NewGuid(),
            TripId = Guid.NewGuid(),
            Latitude = 2.0469,
            Longitude = 45.3182
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*not found*");
    }

    [Fact]
    public void Command_CanSetLatitudeAndLongitude()
    {
        // Arrange & Act
        var command = new DetectRouteDeviationCommand
        {
            TenantId = Guid.NewGuid(),
            TripId = Guid.NewGuid(),
            Latitude = 2.0469,
            Longitude = 45.3182
        };

        // Assert
        command.Latitude.Should().Be(2.0469);
        command.Longitude.Should().Be(45.3182);
        command.TenantId.Should().NotBeEmpty();
        command.TripId.Should().NotBeEmpty();
    }

    [Fact]
    public void Command_DefaultsToZeroCoordinates()
    {
        // Arrange & Act
        var command = new DetectRouteDeviationCommand();

        // Assert
        command.Latitude.Should().Be(0);
        command.Longitude.Should().Be(0);
        command.TenantId.Should().BeEmpty();
        command.TripId.Should().BeEmpty();
    }
}

/// <summary>
/// Extension helper to build a mock DbSet from an IQueryable with async query support for EF Core unit testing.
/// </summary>
internal static class MockDbSetExtensions
{
    internal static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        var asyncProvider = new TestAsyncQueryProvider<T>(source.Provider);

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(source.GetEnumerator()));

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(asyncProvider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
        return mockSet;
    }
}

internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var isTask = typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>);
        var itemType = isTask ? typeof(TResult).GetGenericArguments()[0] : typeof(TResult);

        var executionResult = typeof(IQueryProvider)
            .GetMethods()
            .First(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod)
            .MakeGenericMethod(itemType)
            .Invoke(_inner, new object[] { expression });

        if (isTask)
        {
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(itemType)
                .Invoke(null, new[] { executionResult })!;
        }

        return (TResult)executionResult!;
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(_inner.MoveNext());
    }

    public T Current => _inner.Current;
}
