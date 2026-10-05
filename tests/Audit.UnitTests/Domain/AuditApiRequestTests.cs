using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditApiRequest"/>.
/// </summary>
public class AuditApiRequestTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var request = new AuditApiRequest();
        var after  = DateTime.UtcNow.AddSeconds(1);

        request.Id.Should().NotBe(Guid.Empty);
        request.TenantId.Should().BeNull();
        request.AuditEventId.Should().Be(Guid.Empty);
        request.HttpMethod.Should().BeEmpty();
        request.Route.Should().BeEmpty();
        request.EndpointCategory.Should().BeNull();
        request.StatusCode.Should().Be(0);
        request.RequestSizeBytes.Should().BeNull();
        request.ResponseSizeBytes.Should().BeNull();
        request.DurationMs.Should().BeNull();
        request.ClientType.Should().BeNull();
        request.ApiVersion.Should().BeNull();
        request.RateLimitResult.Should().BeNull();
        request.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var request = new AuditApiRequest();
        request.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var request = new AuditApiRequest
        {
            TenantId          = Guid.NewGuid(),
            AuditEventId      = Guid.NewGuid(),
            HttpMethod        = "POST",
            Route             = "/api/declarations",
            EndpointCategory  = "Customs",
            StatusCode        = 201,
            RequestSizeBytes  = 1024,
            ResponseSizeBytes = 256,
            DurationMs        = 42,
            ClientType        = "Web",
            ApiVersion        = "v1",
            RateLimitResult   = "Allowed"
        };

        request.HttpMethod.Should().Be("POST");
        request.Route.Should().Be("/api/declarations");
        request.EndpointCategory.Should().Be("Customs");
        request.StatusCode.Should().Be(201);
        request.RequestSizeBytes.Should().Be(1024);
        request.ResponseSizeBytes.Should().Be(256);
        request.DurationMs.Should().Be(42);
        request.ClientType.Should().Be("Web");
        request.ApiVersion.Should().Be("v1");
        request.RateLimitResult.Should().Be("Allowed");
    }
}
