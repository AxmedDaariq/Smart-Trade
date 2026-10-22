using System;
using AisVessel.Domain.Entities;

namespace AisVessel.Tests.Domain.Entities;

public class BerthAssignmentTests
{
    [Fact]
    public void NewBerthAssignment_ShouldDefaultToScheduledStatus()
    {
        // Arrange & Act
        var assignment = new BerthAssignment();

        // Assert
        Assert.Equal("Scheduled", assignment.Status);
        Assert.Equal(default, assignment.PlannedStartAtUtc);
        Assert.Null(assignment.PlannedEndAtUtc);
        Assert.Null(assignment.ActualStartAtUtc);
        Assert.Null(assignment.ActualEndAtUtc);
    }

    [Fact]
    public void NewBerthAssignment_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var assignment = new BerthAssignment();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.InRange(assignment.AssignedAtUtc, before, after);
        Assert.InRange(assignment.CreatedAtUtc, before, after);
        Assert.InRange(assignment.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewBerthAssignments_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new BerthAssignment();
        var second = new BerthAssignment();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void BerthAssignmentProperties_ShouldBeSettable()
    {
        // Arrange
        var assignment = new BerthAssignment();
        var plannedStart = new DateTime(2026, 4, 1, 6, 0, 0, DateTimeKind.Utc);
        var actualStart = new DateTime(2026, 4, 1, 6, 45, 0, DateTimeKind.Utc);

        // Act
        assignment.Status = "Occupied";
        assignment.PlannedStartAtUtc = plannedStart;
        assignment.ActualStartAtUtc = actualStart;
        assignment.AssignedBy = Guid.NewGuid();

        // Assert
        Assert.Equal("Occupied", assignment.Status);
        Assert.Equal(plannedStart, assignment.PlannedStartAtUtc);
        Assert.Equal(actualStart, assignment.ActualStartAtUtc);
        Assert.NotEqual(Guid.Empty, assignment.AssignedBy);
    }
}