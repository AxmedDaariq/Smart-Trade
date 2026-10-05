using System;
using System.Linq;
using AnprGate.Domain.Enums;

namespace AnprGate.Tests.Domain.Enums;

public class EnumsTests
{
    [Theory]
    [InlineData(CameraStatus.Provisioning)]
    [InlineData(CameraStatus.Active)]
    [InlineData(CameraStatus.Offline)]
    [InlineData(CameraStatus.Degraded)]
    [InlineData(CameraStatus.Maintenance)]
    [InlineData(CameraStatus.Suspended)]
    [InlineData(CameraStatus.Retired)]
    public void CameraStatus_ShouldContainKnownStates(CameraStatus status)
    {
        Assert.True(Enum.IsDefined(status));
    }

    [Fact]
    public void CameraStatus_Provisioning_ShouldBeTheDefaultZeroValue()
    {
        Assert.Equal(0, (int)CameraStatus.Provisioning);
    }

    [Theory]
    [InlineData(CameraType.ANPR)]
    [InlineData(CameraType.Overview)]
    [InlineData(CameraType.Lane)]
    [InlineData(CameraType.Entry)]
    [InlineData(CameraType.Exit)]
    [InlineData(CameraType.Border)]
    [InlineData(CameraType.PlateCapture)]
    [InlineData(CameraType.Other)]
    public void CameraType_ShouldContainKnownCameraKinds(CameraType cameraType)
    {
        Assert.True(Enum.IsDefined(cameraType));
    }

    [Fact]
    public void GateStatus_Active_ShouldBeTheDefaultZeroValue()
    {
        // Assert
        Assert.Equal(0, (int)GateStatus.Active);
        Assert.Equal(GateStatus.Closed, Enum.Parse<GateStatus>("Closed"));
        Assert.Equal(GateStatus.EmergencyClosed, Enum.Parse<GateStatus>("EmergencyClosed"));
        Assert.False(Enum.IsDefined(typeof(GateStatus), 99));
    }

    [Theory]
    [InlineData(GateType.PortEntry)]
    [InlineData(GateType.PortExit)]
    [InlineData(GateType.BorderEntry)]
    [InlineData(GateType.BorderExit)]
    [InlineData(GateType.DryPortEntry)]
    [InlineData(GateType.Checkpoint)]
    [InlineData(GateType.Other)]
    public void GateType_ShouldContainKnownGateKinds(GateType gateType)
    {
        Assert.True(Enum.IsDefined(gateType));
    }

    [Fact]
    public void RecognitionStatus_ShouldCoverTheRecognitionLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)RecognitionStatus.Detected);
        Assert.Equal(RecognitionStatus.Validated, Enum.Parse<RecognitionStatus>("Validated"));
        Assert.Equal(RecognitionStatus.Matched, Enum.Parse<RecognitionStatus>("Matched"));
        Assert.Equal(RecognitionStatus.Unmatched, Enum.Parse<RecognitionStatus>("Unmatched"));
        Assert.Equal(RecognitionStatus.ManualReview, Enum.Parse<RecognitionStatus>("ManualReview"));
        Assert.Equal(RecognitionStatus.Expired, Enum.Parse<RecognitionStatus>("Expired"));
    }

    [Fact]
    public void AuthorizationStatus_ShouldCoverTheAuthorizationLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)AuthorizationStatus.Requested);
        Assert.Equal(AuthorizationStatus.Evaluating, Enum.Parse<AuthorizationStatus>("Evaluating"));
        Assert.Equal(AuthorizationStatus.Authorized, Enum.Parse<AuthorizationStatus>("Authorized"));
        Assert.Equal(AuthorizationStatus.Denied, Enum.Parse<AuthorizationStatus>("Denied"));
        Assert.Equal(AuthorizationStatus.Expired, Enum.Parse<AuthorizationStatus>("Expired"));
        Assert.Equal(AuthorizationStatus.Cancelled, Enum.Parse<AuthorizationStatus>("Cancelled"));
    }

    [Fact]
    public void PassageStatus_ShouldContainEightStates()
    {
        Assert.Equal(8, Enum.GetValues<PassageStatus>().Length);
        Assert.Equal(PassageStatus.PendingAuthorization, Enum.Parse<PassageStatus>("PendingAuthorization"));
        Assert.Equal(PassageStatus.Exception, Enum.Parse<PassageStatus>("Exception"));
    }

    [Fact]
    public void BorderCrossingStatus_ShouldCoverTheCrossingLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)BorderCrossingStatus.Approaching);
        Assert.Equal(BorderCrossingStatus.EntryDetected, Enum.Parse<BorderCrossingStatus>("EntryDetected"));
        Assert.Equal(BorderCrossingStatus.ExitDetected, Enum.Parse<BorderCrossingStatus>("ExitDetected"));
        Assert.Equal(BorderCrossingStatus.Completed, Enum.Parse<BorderCrossingStatus>("Completed"));
        Assert.Equal(BorderCrossingStatus.Denied, Enum.Parse<BorderCrossingStatus>("Denied"));
    }

    [Fact]
    public void Direction_ShouldHaveInboundAsZeroAndContainFourStates()
    {
        // Assert: direction drives lane/flow logic, ordering of Inbound first is relied upon
        Assert.Equal(0, (int)Direction.Inbound);
        Assert.Equal(4, Enum.GetValues<Direction>().Length);
        Assert.Equal(Direction.Outbound, Enum.Parse<Direction>("Outbound"));
        Assert.Equal(Direction.Bidirectional, Enum.Parse<Direction>("Bidirectional"));
        Assert.Equal(Direction.Unknown, Enum.Parse<Direction>("Unknown"));
    }

    [Fact]
    public void AllEnums_ShouldHaveNoDuplicateNames()
    {
        // Assert: duplicate names would break string-based persistence/serialization
        AssertNoDuplicateNames<CameraStatus>();
        AssertNoDuplicateNames<CameraType>();
        AssertNoDuplicateNames<GateStatus>();
        AssertNoDuplicateNames<GateType>();
        AssertNoDuplicateNames<RecognitionStatus>();
        AssertNoDuplicateNames<AuthorizationStatus>();
        AssertNoDuplicateNames<PassageStatus>();
        AssertNoDuplicateNames<BorderCrossingStatus>();
        AssertNoDuplicateNames<Direction>();
    }

    private static void AssertNoDuplicateNames<T>() where T : struct, Enum
    {
        var names = Enum.GetNames<T>();
        Assert.Equal(names.Length, names.Distinct().Count());
    }
}