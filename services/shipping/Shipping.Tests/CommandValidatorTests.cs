using Shipping.Application.Commands;
using Shipping.Domain.Enums;

namespace Shipping.Tests;

public class CommandValidatorTests
{
    private static CreateShipmentCommand ValidCreateCommand() => new()
    {
        TenantId = Guid.NewGuid(),
        ShipperName = "Acme Exports Ltd",
        ShipperReference = "ACME-001",
        ConsigneeName = "Global Imports Co",
        ConsigneeReference = "GI-001",
        OriginCountryCode = "SO",
        OriginPortCode = "SOBER",
        DestinationCountryCode = "AE",
        DestinationPortCode = "AEJEA",
        TransportMode = TransportMode.Sea,
        CargoType = CargoType.FCL,
        Incoterm = Incoterm.FOB,
        Currency = "USD",
        GrossWeightKg = 1000,
        PackageCount = 10,
        UserId = Guid.NewGuid()
    };

    [Fact]
    public void CreateShipmentCommandValidator_ValidCommand_Passes()
    {
        var validator = new CreateShipmentCommandValidator();
        var result = validator.Validate(ValidCreateCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateShipmentCommandValidator_MissingTenant_Fails()
    {
        var command = ValidCreateCommand();
        command.TenantId = Guid.Empty;

        var result = new CreateShipmentCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.TenantId));
    }

    [Fact]
    public void CreateShipmentCommandValidator_EmptyShipperName_Fails()
    {
        var command = ValidCreateCommand();
        command.ShipperName = string.Empty;

        var result = new CreateShipmentCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ShipperName));
    }

    [Fact]
    public void CreateShipmentCommandValidator_InvalidCountryCodeLength_Fails()
    {
        var command = ValidCreateCommand();
        command.OriginCountryCode = "SOMALIA"; // longer than 3 chars

        var result = new CreateShipmentCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.OriginCountryCode));
    }

    [Fact]
    public void CreateShipmentCommandValidator_NonPositiveWeight_Fails()
    {
        var command = ValidCreateCommand();
        command.GrossWeightKg = 0;

        var result = new CreateShipmentCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.GrossWeightKg));
    }

    [Fact]
    public void CreateShipmentCommandValidator_ZeroPackageCount_Fails()
    {
        var command = ValidCreateCommand();
        command.PackageCount = 0;

        var result = new CreateShipmentCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.PackageCount));
    }

    [Fact]
    public void UpdateShipmentStatusCommandValidator_ValidCommand_Passes()
    {
        var validator = new UpdateShipmentStatusCommandValidator();
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            NewStatus = ShipmentStatus.Booked,
            Reason = "Booking confirmed",
            UserId = Guid.NewGuid()
        };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateShipmentStatusCommandValidator_MissingUser_Fails()
    {
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            NewStatus = ShipmentStatus.Booked,
            UserId = Guid.Empty
        };

        var result = new UpdateShipmentStatusCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.UserId));
    }

    [Fact]
    public void UpdateShipmentStatusCommandValidator_ReasonTooLong_Fails()
    {
        var command = new UpdateShipmentStatusCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            NewStatus = ShipmentStatus.OnHold,
            Reason = new string('x', 501),
            UserId = Guid.NewGuid()
        };

        var result = new UpdateShipmentStatusCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Reason));
    }

    [Fact]
    public void RecordTrackingEventCommandValidator_ValidCommand_Passes()
    {
        var validator = new RecordTrackingEventCommandValidator();
        var command = new RecordTrackingEventCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            EventType = TrackingEventType.CargoPickedUp,
            Location = "Berbera Port",
            Latitude = 10.4,
            Longitude = 45.0,
            UserId = Guid.NewGuid()
        };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RecordTrackingEventCommandValidator_EmptyLocation_Fails()
    {
        var command = new RecordTrackingEventCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            EventType = TrackingEventType.OrderCreated,
            Location = string.Empty,
            UserId = Guid.NewGuid()
        };

        var result = new RecordTrackingEventCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Location));
    }

    [Fact]
    public void RecordTrackingEventCommandValidator_InvalidLatitude_Fails()
    {
        var command = new RecordTrackingEventCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            EventType = TrackingEventType.OrderCreated,
            Location = "Berbera",
            Latitude = 91.0, // out of range
            UserId = Guid.NewGuid()
        };

        var result = new RecordTrackingEventCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Latitude));
    }

    [Fact]
    public void RecordTrackingEventCommandValidator_InvalidLongitude_Fails()
    {
        var command = new RecordTrackingEventCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            EventType = TrackingEventType.OrderCreated,
            Location = "Berbera",
            Longitude = -181.0, // out of range
            UserId = Guid.NewGuid()
        };

        var result = new RecordTrackingEventCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Longitude));
    }

    [Fact]
    public void ConfirmCustomsClearanceCommandValidator_ValidCommand_Passes()
    {
        var validator = new ConfirmCustomsClearanceCommandValidator();
        var command = new ConfirmCustomsClearanceCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            CustomsCaseId = Guid.NewGuid(),
            CustomsCaseNumber = "CUS-2024-0001",
            UserId = Guid.NewGuid()
        };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ConfirmCustomsClearanceCommandValidator_MissingCaseNumber_Fails()
    {
        var command = new ConfirmCustomsClearanceCommand
        {
            TenantId = Guid.NewGuid(),
            ShipmentId = Guid.NewGuid(),
            CustomsCaseId = Guid.NewGuid(),
            CustomsCaseNumber = string.Empty,
            UserId = Guid.NewGuid()
        };

        var result = new ConfirmCustomsClearanceCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CustomsCaseNumber));
    }
}
