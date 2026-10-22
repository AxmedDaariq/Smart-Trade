using System;
using System.Linq;
using Ectn.Domain.Entities;
using Xunit;

namespace Ectn.Tests.Entities;

public class EctnApplicationTests
{
    [Fact]
    public void EctnApplication_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var application = new EctnApplication();

        // Assert
        Assert.NotEqual(Guid.Empty, application.Id);
        Assert.Equal("Draft", application.Status);
        Assert.Empty(application.ApplicationNumber);
        Assert.Empty(application.ApplicationType);
        
        // Assert dates are set
        Assert.True(application.CreatedAtUtc <= DateTime.UtcNow);
        Assert.True(application.UpdatedAtUtc <= DateTime.UtcNow);
        
        // Assert collections are initialized
        Assert.NotNull(application.Certificates);
        Assert.NotNull(application.Shipments);
        Assert.NotNull(application.PartyRoles);
        Assert.NotNull(application.Documents);
        Assert.NotNull(application.StatusHistories);
        
        Assert.Empty(application.Certificates);
        Assert.Empty(application.Shipments);
        Assert.Empty(application.PartyRoles);
        Assert.Empty(application.Documents);
        Assert.Empty(application.StatusHistories);
    }

    [Fact]
    public void EctnApplication_Can_Add_Status_History()
    {
        // Arrange
        var application = new EctnApplication();
        var statusHistory = new EctnStatusHistory
        {
            Status = Domain.Enums.EctnStatus.Submitted,
            FromStatus = "Draft",
            ToStatus = "Submitted",
            Reason = "User submitted the application"
        };

        // Act
        application.StatusHistories.Add(statusHistory);

        // Assert
        Assert.Single(application.StatusHistories);
        Assert.Equal("Submitted", application.StatusHistories.First().ToStatus);
    }
}
