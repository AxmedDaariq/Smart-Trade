using System;
using Fleet.Domain.Entities;
using Xunit;

namespace Fleet.Tests.Entities;

public class VehicleTests
{
    [Fact]
    public void Vehicle_Should_Initialize_With_Correct_Defaults()
    {
        // Arrange & Act
        var vehicle = new Vehicle
        {
            TenantId = Guid.NewGuid(),
            FleetCompanyId = Guid.NewGuid(),
            VehicleReference = "VEH-2026-001",
            VehicleType = "Truck",
            VehicleCategory = "Heavy",
            Make = "Mercedes-Benz",
            Model = "Actros",
            ModelYear = 2023,
            VIN = "WDB93403211234567",
            LicensePlate = "DJ-1234",
            NormalizedLicensePlate = "DJ1234",
            FuelType = "Diesel",
            OwnershipType = "Owned",
            CountryOfRegistration = "SO",
            CreatedBy = Guid.NewGuid()
        };

        // Assert
        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.Equal("Draft", vehicle.Status);       // Default status
        Assert.Equal("Truck", vehicle.VehicleType);
        Assert.Equal("Heavy", vehicle.VehicleCategory);
        Assert.Equal("Mercedes-Benz", vehicle.Make);
        Assert.Equal("Diesel", vehicle.FuelType);
        Assert.Equal("SO", vehicle.CountryOfRegistration);

        // Collections should be initialized and empty
        Assert.NotNull(vehicle.Registrations);
        Assert.Empty(vehicle.Registrations);
        Assert.NotNull(vehicle.Compliances);
        Assert.Empty(vehicle.Compliances);
        Assert.NotNull(vehicle.Inspections);
        Assert.Empty(vehicle.Inspections);
        Assert.NotNull(vehicle.Documents);
        Assert.Empty(vehicle.Documents);
        Assert.NotNull(vehicle.Capabilities);
        Assert.Empty(vehicle.Capabilities);
        Assert.NotNull(vehicle.Assignments);
        Assert.Empty(vehicle.Assignments);
    }

    [Fact]
    public void Vehicle_LicensePlate_Should_Be_Stored_As_Provided()
    {
        // Arrange & Act
        var vehicle = new Vehicle
        {
            LicensePlate = "DJ-5678",
            NormalizedLicensePlate = "DJ5678"
        };

        // Assert
        Assert.Equal("DJ-5678", vehicle.LicensePlate);
        Assert.Equal("DJ5678", vehicle.NormalizedLicensePlate);
    }

    [Fact]
    public void Vehicle_Status_Should_Default_To_Draft()
    {
        // Act
        var vehicle = new Vehicle();

        // Assert
        Assert.Equal("Draft", vehicle.Status);
    }

    [Fact]
    public void Vehicle_Should_Have_Unique_Id_Per_Instance()
    {
        // Arrange & Act
        var vehicle1 = new Vehicle();
        var vehicle2 = new Vehicle();

        // Assert
        Assert.NotEqual(vehicle1.Id, vehicle2.Id);
    }
}
