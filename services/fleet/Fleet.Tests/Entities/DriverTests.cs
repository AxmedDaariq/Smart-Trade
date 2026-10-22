using System;
using Fleet.Domain.Entities;
using Xunit;

namespace Fleet.Tests.Entities;

public class DriverTests
{
    [Fact]
    public void Driver_Should_Initialize_With_Correct_Defaults()
    {
        // Arrange & Act
        var tenantId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();

        var driver = new Driver
        {
            TenantId = tenantId,
            FleetCompanyId = companyId,
            DriverReference = "DRV-2026-001",
            FirstName = "Axmed",
            LastName = "Cali",
            DisplayName = "Axmed Cali",
            PhoneNumber = "+252612345678",
            Email = "axmed.cali@example.so",
            DateOfBirth = new DateTime(1990, 5, 15),
            Nationality = "Somali",
            CountryCode = "SO",
            CreatedBy = createdBy
        };

        // Assert
        Assert.NotEqual(Guid.Empty, driver.Id);
        Assert.Equal("Pending", driver.Status);               // Default status
        Assert.Equal("Pending", driver.VerificationStatus);  // Default verification
        Assert.Equal(tenantId, driver.TenantId);
        Assert.Equal("Axmed", driver.FirstName);
        Assert.Equal("Cali", driver.LastName);
        Assert.Equal("SO", driver.CountryCode);
        Assert.Null(driver.UserId);      // No user linked by default
        Assert.Null(driver.EmployeeNumber);

        // Collections should be initialized and empty
        Assert.NotNull(driver.Licenses);
        Assert.Empty(driver.Licenses);
        Assert.NotNull(driver.Qualifications);
        Assert.Empty(driver.Qualifications);
        Assert.NotNull(driver.Compliances);
        Assert.Empty(driver.Compliances);
        Assert.NotNull(driver.Verifications);
        Assert.Empty(driver.Verifications);
        Assert.NotNull(driver.Documents);
        Assert.Empty(driver.Documents);
        Assert.NotNull(driver.Capabilities);
        Assert.Empty(driver.Capabilities);
        Assert.NotNull(driver.Assignments);
        Assert.Empty(driver.Assignments);
    }

    [Fact]
    public void Driver_Status_Should_Default_To_Pending()
    {
        // Act
        var driver = new Driver();

        // Assert
        Assert.Equal("Pending", driver.Status);
        Assert.Equal("Pending", driver.VerificationStatus);
    }

    [Fact]
    public void Driver_Should_Have_Unique_Id_Per_Instance()
    {
        // Arrange & Act
        var driver1 = new Driver();
        var driver2 = new Driver();

        // Assert
        Assert.NotEqual(driver1.Id, driver2.Id);
    }

    [Fact]
    public void Driver_DateOfBirth_Should_Be_Stored_Correctly()
    {
        // Arrange
        var dob = new DateTime(1985, 3, 22);

        // Act
        var driver = new Driver { DateOfBirth = dob };

        // Assert
        Assert.Equal(1985, driver.DateOfBirth.Year);
        Assert.Equal(3, driver.DateOfBirth.Month);
        Assert.Equal(22, driver.DateOfBirth.Day);
    }
}
