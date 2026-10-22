using System;
using System.Linq;
using Integration.Domain.Enums;

namespace Integration.Tests.Domain.Enums;

public class EnumsTests
{
    [Theory]
    [InlineData(SystemType.Government)]
    [InlineData(SystemType.Customs)]
    [InlineData(SystemType.Port)]
    [InlineData(SystemType.Bank)]
    [InlineData(SystemType.MobileMoney)]
    [InlineData(SystemType.ECTN)]
    [InlineData(SystemType.GPSProvider)]
    [InlineData(SystemType.ANPRProvider)]
    [InlineData(SystemType.AISProvider)]
    [InlineData(SystemType.Other)]
    public void SystemType_ShouldContainKnownSystemKinds(SystemType systemType)
    {
        Assert.True(Enum.IsDefined(systemType));
    }

    [Fact]
    public void SystemType_Government_ShouldBeTheDefaultZeroValue()
    {
        Assert.Equal(0, (int)SystemType.Government);
    }

    [Fact]
    public void Direction_ShouldCoverInboundOutboundAndBidirectional()
    {
        // Assert
        Assert.Equal(0, (int)Direction.Inbound);
        Assert.Equal(1, (int)Direction.Outbound);
        Assert.Equal(2, (int)Direction.Bidirectional);
        Assert.True(Enum.IsDefined(Direction.Bidirectional));
    }

    [Theory]
    [InlineData(Protocol.REST)]
    [InlineData(Protocol.Webhook)]
    [InlineData(Protocol.SOAP)]
    [InlineData(Protocol.SFTP)]
    [InlineData(Protocol.AMQP)]
    [InlineData(Protocol.gRPC)]
    [InlineData(Protocol.Other)]
    public void Protocol_ShouldContainSupportedProtocols(Protocol protocol)
    {
        Assert.True(Enum.IsDefined(protocol));
    }

    [Fact]
    public void PartnerStatus_ShouldCoverThePartnerLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)PartnerStatus.Pending);
        Assert.Equal(PartnerStatus.Pending, Enum.Parse<PartnerStatus>("Pending"));
        Assert.Equal(PartnerStatus.Active, Enum.Parse<PartnerStatus>("Active"));
        Assert.Equal(PartnerStatus.Suspended, Enum.Parse<PartnerStatus>("Suspended"));
        Assert.Equal(PartnerStatus.Inactive, Enum.Parse<PartnerStatus>("Inactive"));
        Assert.False(Enum.IsDefined(typeof(PartnerStatus), 99));
    }

    [Fact]
    public void IntegrationStatus_ShouldCoverTheIntegrationLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)IntegrationStatus.Draft);
        Assert.Equal(IntegrationStatus.Draft, Enum.Parse<IntegrationStatus>("Draft"));
        Assert.Equal(IntegrationStatus.Active, Enum.Parse<IntegrationStatus>("Active"));
        Assert.Equal(IntegrationStatus.Deprecated, Enum.Parse<IntegrationStatus>("Deprecated"));
        Assert.Equal(IntegrationStatus.Retired, Enum.Parse<IntegrationStatus>("Retired"));
        Assert.Equal(4, Enum.GetValues<IntegrationStatus>().Length);
    }

    [Fact]
    public void AllEnums_ShouldHaveNoDuplicateNames()
    {
        // Assert: duplicate names would break string-based persistence/serialization
        AssertNoDuplicateNames<SystemType>();
        AssertNoDuplicateNames<Direction>();
        AssertNoDuplicateNames<Protocol>();
        AssertNoDuplicateNames<PartnerStatus>();
        AssertNoDuplicateNames<IntegrationStatus>();
    }

    private static void AssertNoDuplicateNames<T>() where T : struct, Enum
    {
        var names = Enum.GetNames<T>();
        Assert.Equal(names.Length, names.Distinct().Count());
    }
}
