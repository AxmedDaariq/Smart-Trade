using System;
using Identity.Domain.Entities;
using Xunit;

namespace Identity.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void User_Should_Initialize_With_Default_Values()
    {
        // Act
        var user = new User();

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Pending", user.Status);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(0, user.FailedLoginCount);
        Assert.False(user.MfaEnabled);
        Assert.True(user.CreatedAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void User_Should_Update_Properties_Correctly()
    {
        // Arrange
        var user = new User();
        var tenantId = Guid.NewGuid();

        // Act
        user.TenantId = tenantId;
        user.Username = "admin";
        user.Email = "admin@example.com";
        user.Status = "Active";

        // Assert
        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("admin", user.Username);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal("Active", user.Status);
    }
}
