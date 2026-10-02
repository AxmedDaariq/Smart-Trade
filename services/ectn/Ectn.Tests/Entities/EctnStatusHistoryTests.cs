using System;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;
using Xunit;

namespace Ectn.Tests.Entities;

public class EctnStatusHistoryTests
{
    [Fact]
    public void EctnStatusHistory_Should_Initialize_With_New_Id_And_Current_Utc_Time()
    {
        // Arrange & Act
        var statusHistory = new EctnStatusHistory();

        // Assert
        Assert.NotEqual(Guid.Empty, statusHistory.Id);
        Assert.True(statusHistory.ChangedAtUtc <= DateTime.UtcNow);
        Assert.True(statusHistory.ChangedAtUtc >= DateTime.UtcNow.AddSeconds(-2));
        
        Assert.Equal(statusHistory.ChangedAt, statusHistory.ChangedAtUtc);
    }

    [Fact]
    public void ChangedAt_Property_Should_Update_ChangedAtUtc()
    {
        // Arrange
        var statusHistory = new EctnStatusHistory();
        var newTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        statusHistory.ChangedAt = newTime;

        // Assert
        Assert.Equal(newTime, statusHistory.ChangedAtUtc);
        Assert.Equal(newTime, statusHistory.ChangedAt);
    }

    [Fact]
    public void EctnStatusHistory_Should_Store_Status_Transitions_Correctly()
    {
        // Arrange
        var statusHistory = new EctnStatusHistory
        {
            Status = EctnStatus.Approved,
            FromStatus = "Pending",
            ToStatus = "Approved",
            Reason = "All documents verified"
        };

        // Assert
        Assert.Equal(EctnStatus.Approved, statusHistory.Status);
        Assert.Equal("Pending", statusHistory.FromStatus);
        Assert.Equal("Approved", statusHistory.ToStatus);
        Assert.Equal("All documents verified", statusHistory.Reason);
    }
}
