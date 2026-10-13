using System;
using System.Collections.Generic;
using Customs.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CustomsCaseNumberGenerator"/>.
/// </summary>
public sealed class CustomsCaseNumberGeneratorTests
{
    private readonly CustomsCaseNumberGenerator _generator = new();

    [Fact]
    public void Generate_ReturnsFormattedCaseNumber_StartingWithPrefix()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var result = _generator.Generate(tenantId);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().StartWith("CC-");
    }

    [Fact]
    public void Generate_ProducesUniqueNumbers_OnSequentialCalls()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var numbers = new HashSet<string>();

        // Act
        for (var i = 0; i < 50; i++)
        {
            var num = _generator.Generate(tenantId);
            numbers.Add(num);
        }

        // Assert: All 50 case numbers must be unique
        numbers.Count.Should().Be(50);
    }

    [Fact]
    public void Generate_ContainsCurrentDateComponent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var today = DateTime.UtcNow.ToString("yyyyMMdd");

        // Act
        var result = _generator.Generate(tenantId);

        // Assert
        result.Should().Contain(today);
    }
}
