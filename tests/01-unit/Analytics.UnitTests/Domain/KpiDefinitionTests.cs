using Xunit;
using FluentAssertions;
using Analytics.Domain.Entities;

namespace Analytics.UnitTests.Domain;

public class KpiDefinitionTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var code = "avg-transit-time";
        var name = "Average Transit Time";
        var category = "Efficiency";
        var targetValue = 24.0m;
        var warningThreshold = 26.0m;
        var criticalThreshold = 30.0m;
        var unit = "Hours";

        // Act
        var kpi = new KpiDefinition(code, name, category, targetValue, warningThreshold, criticalThreshold, unit);

        // Assert
        kpi.Code.Should().Be(code);
        kpi.Name.Should().Be(name);
        kpi.Category.Should().Be(category);
        kpi.TargetValue.Should().Be(targetValue);
        kpi.WarningThreshold.Should().Be(warningThreshold);
        kpi.CriticalThreshold.Should().Be(criticalThreshold);
        kpi.Unit.Should().Be(unit);
        kpi.FormulaVersion.Should().Be("v1.0");
    }
}
