using System;
using System.Linq;
using AnprGate.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace AnprGate.UnitTests.Domain;

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
        Enum.IsDefined(status).Should().BeTrue();
    }

    [Fact]
    public void CameraStatus_Provisioning_ShouldBeTheDefaultZeroValue()
    {
        ((int)CameraStatus.Provisioning).Should().Be(0);
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
        Enum.IsDefined(cameraType).Should().BeTrue();
    }

    [Fact]
    public void GateStatus_Active_ShouldBeTheDefaultZeroValue()
    {
        ((int)GateStatus.Active).Should().Be(0);
        Enum.Parse<GateStatus>("Closed").Should().Be(GateStatus.Closed);
        Enum.Parse<GateStatus>("EmergencyClosed").Should().Be(GateStatus.EmergencyClosed);
        Enum.IsDefined(typeof(GateStatus), 99).Should().BeFalse();
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
        Enum.IsDefined(gateType).Should().BeTrue();
    }

    [Fact]
    public void RecognitionStatus_ShouldCoverTheRecognitionLifecycle()
    {
        ((int)RecognitionStatus.Detected).Should().Be(0);
        Enum.Parse<RecognitionStatus>("Validated").Should().Be(RecognitionStatus.Validated);
        Enum.Parse<RecognitionStatus>("Matched").Should().Be(RecognitionStatus.Matched);
        Enum.Parse<RecognitionStatus>("Unmatched").Should().Be(RecognitionStatus.Unmatched);
        Enum.Parse<RecognitionStatus>("ManualReview").Should().Be(RecognitionStatus.ManualReview);
        Enum.Parse<RecognitionStatus>("Expired").Should().Be(RecognitionStatus.Expired);
    }

    [Fact]
    public void AuthorizationStatus_ShouldCoverTheAuthorizationLifecycle()
    {
        ((int)AuthorizationStatus.Requested).Should().Be(0);
        Enum.Parse<AuthorizationStatus>("Evaluating").Should().Be(AuthorizationStatus.Evaluating);
        Enum.Parse<AuthorizationStatus>("Authorized").Should().Be(AuthorizationStatus.Authorized);
        Enum.Parse<AuthorizationStatus>("Denied").Should().Be(AuthorizationStatus.Denied);
        Enum.Parse<AuthorizationStatus>("Expired").Should().Be(AuthorizationStatus.Expired);
        Enum.Parse<AuthorizationStatus>("Cancelled").Should().Be(AuthorizationStatus.Cancelled);
    }

    [Fact]
    public void PassageStatus_ShouldContainEightStates()
    {
        Enum.GetValues<PassageStatus>().Length.Should().Be(8);
        Enum.Parse<PassageStatus>("PendingAuthorization").Should().Be(PassageStatus.PendingAuthorization);
        Enum.Parse<PassageStatus>("Exception").Should().Be(PassageStatus.Exception);
    }

    [Fact]
    public void BorderCrossingStatus_ShouldCoverTheCrossingLifecycle()
    {
        ((int)BorderCrossingStatus.Approaching).Should().Be(0);
        Enum.Parse<BorderCrossingStatus>("EntryDetected").Should().Be(BorderCrossingStatus.EntryDetected);
        Enum.Parse<BorderCrossingStatus>("ExitDetected").Should().Be(BorderCrossingStatus.ExitDetected);
        Enum.Parse<BorderCrossingStatus>("Completed").Should().Be(BorderCrossingStatus.Completed);
        Enum.Parse<BorderCrossingStatus>("Denied").Should().Be(BorderCrossingStatus.Denied);
    }

    [Fact]
    public void Direction_ShouldHaveInboundAsZeroAndContainFourStates()
    {
        ((int)Direction.Inbound).Should().Be(0);
        Enum.GetValues<Direction>().Length.Should().Be(4);
        Enum.Parse<Direction>("Outbound").Should().Be(Direction.Outbound);
        Enum.Parse<Direction>("Bidirectional").Should().Be(Direction.Bidirectional);
        Enum.Parse<Direction>("Unknown").Should().Be(Direction.Unknown);
    }

    [Fact]
    public void AllEnums_ShouldHaveNoDuplicateNames()
    {
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
        names.Length.Should().Be(names.Distinct().Count());
    }
}
