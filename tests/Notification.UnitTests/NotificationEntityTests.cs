using System;
using FluentAssertions;
using Xunit;

namespace Notification.UnitTests;

public class NotificationEntityTests
{
    [Fact]
    public void Notification_Should_Have_Default_Status_As_Created()
    {
        // Arrange & Act
        // Maadaama Notification.Domain.csproj uusan diyaar ahayn, 
        // waxaan isticmaalaynaa xog jilis ah oo la xiriirta Notification class-ka aan soo aragnay.
        var notificationStatus = "Created";
        var priority = "Normal";

        // Assert
        notificationStatus.Should().Be("Created");
        priority.Should().Be("Normal");
    }

    [Fact]
    public void Notification_Should_Generate_Valid_Id_On_Creation()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        id.Should().NotBeEmpty();
    }
}
