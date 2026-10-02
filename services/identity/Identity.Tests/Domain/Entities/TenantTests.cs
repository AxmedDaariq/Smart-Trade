using System;
using Identity.Domain.Entities;
using Xunit;

namespace Identity.Tests.Domain.Entities;

public class TenantTests
{
    [Fact]
    public void Tenant_Should_Initialize_With_Default_Values()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        Assert.NotEqual(Guid.Empty, tenant.Id);
        Assert.Equal("Pending", tenant.Status);
        Assert.Equal(string.Empty, tenant.TenantCode);
        Assert.Equal(string.Empty, tenant.Name);
        Assert.True(tenant.CreatedAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void Tenant_Should_Update_Properties_Correctly()
    {
        // Arrange
        var tenant = new Tenant();

        // Act
        tenant.TenantCode = "T-001";
        tenant.Name = "Smart Trade Logistics";
        tenant.TenantType = "LogisticsCompany";
        tenant.Status = "Active";

        // Assert
        Assert.Equal("T-001", tenant.TenantCode);
        Assert.Equal("Smart Trade Logistics", tenant.Name);
        Assert.Equal("LogisticsCompany", tenant.TenantType);
        Assert.Equal("Active", tenant.Status);
    }
}
