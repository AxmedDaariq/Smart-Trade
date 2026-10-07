using Shipping.Domain.Entities;
using Shipping.Domain.Enums;

namespace Shipping.Tests;

public class ShipmentDomainTests
{
    private static Shipment CreateShipment() => new(
        tenantId: Guid.NewGuid(),
        shipmentNumber: "SHP-TEST-000001",
        shipperName: "Acme Exports",
        shipperReference: "ACME-001",
        consigneeName: "Global Imports",
        consigneeReference: "GI-001",
        originCountryCode: "SO",
        originPortCode: "SOBER",
        originPortName: "Berbera",
        destinationCountryCode: "AE",
        destinationPortCode: "AEJEA",
        destinationPortName: "Jebel Ali",
        transportMode: TransportMode.Sea,
        cargoType: CargoType.FCL,
        incoterm: Incoterm.FOB,
        currency: "USD",
        createdBy: Guid.NewGuid());

    [Fact]
    public void Constructor_SetsDraftStatusAndAuditFields()
    {
        var shipment = CreateShipment();

        Assert.Equal(ShipmentStatus.Draft, shipment.Status);
        Assert.NotEqual(Guid.Empty, shipment.Id);
        Assert.NotEqual(Guid.Empty, shipment.CreatedBy);
        Assert.NotEqual(default, shipment.CreatedAtUtc);
    }

    [Fact]
    public void TransitionStatus_RecordsHistoryAndUpdatesStatus()
    {
        var shipment = CreateShipment();
        var userId = Guid.NewGuid();

        shipment.TransitionStatus(ShipmentStatus.Booked, "Booking confirmed", userId);

        Assert.Equal(ShipmentStatus.Booked, shipment.Status);
        Assert.Equal("Booking confirmed", shipment.StatusReason);

        var history = Assert.Single(shipment.StatusHistories);
        Assert.Equal("Draft", history.FromStatus);
        Assert.Equal("Booked", history.ToStatus);
        Assert.Equal("Booking confirmed", history.Reason);
        Assert.Equal(userId, history.ChangedBy);
        Assert.NotEqual(default, history.ChangedAtUtc);
    }

    [Fact]
    public void TransitionStatus_ChainsMultipleTransitions()
    {
        var shipment = CreateShipment();
        var userId = Guid.NewGuid();

        shipment.TransitionStatus(ShipmentStatus.Booked, null, userId);
        shipment.TransitionStatus(ShipmentStatus.PickedUp, "Collected", userId);

        Assert.Equal(ShipmentStatus.PickedUp, shipment.Status);
        Assert.Equal(2, shipment.StatusHistories.Count);

        Assert.Equal("Draft", shipment.StatusHistories[0].FromStatus);
        Assert.Equal("Booked", shipment.StatusHistories[0].ToStatus);

        Assert.Equal("Booked", shipment.StatusHistories[1].FromStatus);
        Assert.Equal("PickedUp", shipment.StatusHistories[1].ToStatus);
    }

    [Fact]
    public void AddTrackingEvent_AddsEventWithDetails()
    {
        var shipment = CreateShipment();
        var userId = Guid.NewGuid();

        shipment.AddTrackingEvent(
            TrackingEventType.ArrivedAtOriginTerminal,
            "Berbera Port",
            "Cargo received at terminal",
            userId);

        var evt = Assert.Single(shipment.TrackingEvents);
        Assert.Equal(shipment.Id, evt.ShipmentId);
        Assert.Equal(TrackingEventType.ArrivedAtOriginTerminal, evt.EventType);
        Assert.Equal("Berbera Port", evt.Location);
        Assert.Equal("Cargo received at terminal", evt.Description);
        Assert.Equal(userId, evt.RecordedBy);
        Assert.NotEqual(default, evt.OccurredAtUtc);
    }

    [Fact]
    public void DomainMethods_SetUpdatedAtAndUpdatedBy()
    {
        var shipment = CreateShipment();
        var createdAt = shipment.CreatedAtUtc;
        var userId = Guid.NewGuid();

        shipment.TransitionStatus(ShipmentStatus.Booked, null, userId);

        Assert.Equal(userId, shipment.UpdatedBy);
        Assert.True(shipment.UpdatedAtUtc >= createdAt);
    }
}
