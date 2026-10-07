using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Shipping.Api.IntegrationTests;

namespace Shipping.Api.IntegrationTests;

/// <summary>
/// End-to-end tests over the real HTTP pipeline (routing, JWT auth,
/// authorization policies, MediatR handlers, EF Core + PostgreSQL).
/// Runs against the shared PostgreSQL Testcontainer.
/// </summary>
[Collection("shipping-postgres")]
public class ShipmentsApiIntegrationTests
{
    private static readonly Guid TenantA = Guid.Parse("11111111-0000-0000-0000-000000000001");
    private static readonly Guid TenantB = Guid.Parse("22222222-0000-0000-0000-000000000002");
    private static readonly Guid UserA = Guid.Parse("33333333-0000-0000-0000-000000000003");
    private static readonly Guid UserB = Guid.Parse("44444444-0000-0000-0000-000000000004");

    private readonly HttpClient _client;

    public ShipmentsApiIntegrationTests(PostgresTestContainerFixture db)
    {
        var factory = new ShippingApiFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private string Token(Guid tenant, Guid user, params string[] permissions) =>
        JwtTestTokenFactory.Generate(tenant, user, permissions);

    private static StringContent Json(object body) =>
        new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

    private static StringContent CreateShipmentBody(string shipperReference) => Json(new
    {
        shipperName = "Acme Exports Ltd",
        shipperReference,
        consigneeName = "Global Imports Co",
        consigneeReference = "GI-IT-555",
        originCountryCode = "SO",
        originPortCode = "SOBER",
        originPortName = "Berbera",
        destinationCountryCode = "AE",
        destinationPortCode = "AEJEA",
        destinationPortName = "Jebel Ali",
        transportMode = 0, // Sea
        cargoType = 0,     // FCL
        incoterm = 1,      // FCA
        currency = "USD",
        grossWeightKg = 24000,
        netWeightKg = 22500,
        volumeM3 = 28.5,
        packageCount = 480,
        declaredValue = 125000,
        lines = new[]
        {
            new
            {
                lineNumber = 1,
                description = "Sesame seeds, hulled",
                hsCode = "1207.40",
                countryOfOrigin = "SO",
                quantity = 480,
                unitOfMeasure = "BAG",
                grossWeightKg = 24000,
                netWeightKg = 22500,
                unitValue = 260.42,
                totalValue = 125000,
                currency = "USD"
            }
        }
    });

    // ── A01: deny-by-default ─────────────────────────────────────────

    [Fact]
    public async Task HealthLive_WithoutToken_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListShipments_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/shipments");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("REF-001"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_TamperedToken_ReturnsUnauthorized()
    {
        var token = JwtTestTokenFactory.Tamper(Token(TenantA, UserA, "shipping.create"));
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("REF-001"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_ExpiredToken_ReturnsUnauthorized()
    {
        var token = JwtTestTokenFactory.Generate(
            TenantA, UserA, new[] { "shipping.create" }, TimeSpan.FromMinutes(-10));
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("REF-001"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_WithoutPermission_ReturnsForbidden()
    {
        // Authenticated, but the token carries no shipping.create permission.
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.read"));

        var response = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("REF-001"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── Create / read ────────────────────────────────────────────────

    [Fact]
    public async Task CreateShipment_WithValidToken_ReturnsCreated_AndGetReturnsPersistedShipment()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create", "shipping.read"));

        var response = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-001"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.Location is not null);

        var created = JsonSerializer.Deserialize<JsonElement>(
            await response.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        var getResponse = await _client.GetAsync($"/api/v1/shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var shipment = JsonSerializer.Deserialize<JsonElement>(
            await getResponse.Content.ReadAsStreamAsync());

        Assert.Equal("Acme Exports Ltd", shipment.GetProperty("shipperName").GetString());
        Assert.Equal("Draft", shipment.GetProperty("status").GetString());
        Assert.Equal("SOBER", shipment.GetProperty("origin").GetProperty("portCode").GetString());
        Assert.Equal("AEJEA", shipment.GetProperty("destination").GetProperty("portCode").GetString());
        Assert.StartsWith("SHP-", shipment.GetProperty("shipmentNumber").GetString());
        Assert.Equal("1207.40",
            shipment.GetProperty("lines")[0].GetProperty("hsCode").GetString());
    }

    [Fact]
    public async Task GetShipment_MissingTenantClaim_ReturnsForbidden()
    {
        // Token without tenant_id: the tenant must come from the JWT,
        // never from the caller (A01).
        var token = JwtTestTokenFactory.GenerateWithoutTenant(UserA, new[] { "shipping.read" });
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await _client.GetAsync($"/api/v1/shipments/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetShipment_CrossTenant_ReturnsNotFound()
    {
        // Tenant A creates a shipment…
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create"));
        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-002"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        // …and tenant B (with read permission) tries to read it → 404, not 403:
        // the resource is simply invisible to another tenant (A01 tenant isolation).
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantB, UserB, "shipping.read"));

        var response = await _client.GetAsync($"/api/v1/shipments/{shipmentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListShipments_ReturnsOnlyCallersTenantShipments()
    {
        // Two shipments for tenant A…
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create"));
        await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-003"));
        await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-004"));

        // …one for tenant B.
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantB, UserB, "shipping.create"));
        await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("GI-IT-005"));

        // Listing as tenant A must not leak tenant B's shipment.
        // (The list may also contain shipments created by other tests in
        // this collection — tenant A is fixed — so assert membership,
        // not an exact count.)
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.read"));

        var response = await _client.GetAsync("/api/v1/shipments");
        response.EnsureSuccessStatusCode();

        var list = JsonSerializer.Deserialize<JsonElement>(
            await response.Content.ReadAsStreamAsync());

        var items = list.GetProperty("items").EnumerateArray().ToList();
        Assert.Contains(items, i => i.GetProperty("shipperReference").GetString() == "ACME-IT-003");
        Assert.Contains(items, i => i.GetProperty("shipperReference").GetString() == "ACME-IT-004");
        Assert.DoesNotContain(items,
            i => i.GetProperty("shipperReference").GetString() == "GI-IT-005");
        Assert.True(list.GetProperty("total").GetInt32() >= 2);
    }

    [Fact]
    public async Task ListShipments_StatusFilter_ReturnsOnlyMatchingStatus()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create", "shipping.read", "shipping.status.update"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-006"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        // Book it (Booked = 1) — the other Draft shipment stays Draft.
        await _client.PatchAsync($"/api/v1/shipments/{shipmentId}/status",
            Json(new { newStatus = 1, reason = "Booking confirmed" }));

        var drafts = await (await _client.GetAsync("/api/v1/shipments?status=Draft"))
            .Content.ReadFromJsonAsync<JsonElement>();
        var booked = await (await _client.GetAsync("/api/v1/shipments?status=Booked"))
            .Content.ReadFromJsonAsync<JsonElement>();

        Assert.All(drafts.GetProperty("items").EnumerateArray(),
            i => Assert.Equal("Draft", i.GetProperty("status").GetString()));
        Assert.All(booked.GetProperty("items").EnumerateArray(),
            i => Assert.Equal("Booked", i.GetProperty("status").GetString()));
    }

    // ── Status transitions ───────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_ValidTransition_ReturnsNoContent_AndPersistsStatusAndReason()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create", "shipping.read", "shipping.status.update"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-007"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        var patchResponse = await _client.PatchAsync($"/api/v1/shipments/{shipmentId}/status",
            Json(new { newStatus = 1, reason = "Booking confirmed by carrier" }));

        Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

        var shipment = JsonSerializer.Deserialize<JsonElement>(
            await (await _client.GetAsync($"/api/v1/shipments/{shipmentId}")).Content.ReadAsStreamAsync());

        Assert.Equal("Booked", shipment.GetProperty("status").GetString());
        Assert.Equal("Booking confirmed by carrier", shipment.GetProperty("statusReason").GetString());
    }

    [Fact]
    public async Task UpdateStatus_TerminalShipment_ReturnsProblemDetails_WithoutLeakingInternals()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create", "shipping.status.update"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-008"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        // Move to Delivered (11) — a terminal state…
        await _client.PatchAsync($"/api/v1/shipments/{shipmentId}/status",
            Json(new { newStatus = 11 }));

        // …then try to transition again: the domain guard throws and the
        // platform exception handler must answer with a generic 500 ProblemDetails
        // that does NOT expose the exception message or the shipment id (A05/A09).
        var response = await _client.PatchAsync($"/api/v1/shipments/{shipmentId}/status",
            Json(new { newStatus = 5 }));

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("An unexpected error occurred.", body);
        Assert.DoesNotContain("Cannot transition", body);
        Assert.DoesNotContain(shipmentId.ToString(), body);
    }

    // ── Tracking ─────────────────────────────────────────────────────

    [Fact]
    public async Task RecordTrackingEvent_ReturnsCreated_AndTimelineContainsEvent()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA,
                "shipping.create", "shipping.read", "shipping.tracking.record"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-009"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        // CargoPickedUp = 3
        var postResponse = await _client.PostAsync($"/api/v1/shipments/{shipmentId}/tracking",
            Json(new
            {
                eventType = 3,
                location = "Berbera ICD",
                description = "Container collected by carrier",
                latitude = 10.4,
                longitude = 45.0,
                vehicleIdentifier = "TRK-7788",
                carrierEventCode = "PUP-001"
            }));

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var timelineResponse = await _client.GetAsync($"/api/v1/shipments/{shipmentId}/tracking");
        Assert.Equal(HttpStatusCode.OK, timelineResponse.StatusCode);

        var timeline = JsonSerializer.Deserialize<JsonElement>(
            await timelineResponse.Content.ReadAsStreamAsync());

        var evt = Assert.Single(timeline.EnumerateArray());
        Assert.Equal("CargoPickedUp", evt.GetProperty("eventType").GetString());
        Assert.Equal("Berbera ICD", evt.GetProperty("location").GetString());
        Assert.Equal(10.4, evt.GetProperty("latitude").GetDouble());
        Assert.Equal("TRK-7788", evt.GetProperty("vehicleIdentifier").GetString());
    }

    [Fact]
    public async Task GetTrackingTimeline_CrossTenant_ReturnsNotFound()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create"));
        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-010"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantB, UserB, "shipping.read"));

        var response = await _client.GetAsync($"/api/v1/shipments/{shipmentId}/tracking");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── Customs clearance ────────────────────────────────────────────

    [Fact]
    public async Task ConfirmCustomsClearance_ReturnsNoContent_AndMarksShipmentCleared()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA,
                "shipping.create", "shipping.read", "shipping.customs.confirm"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-011"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();
        var customsCaseId = Guid.NewGuid();

        var response = await _client.PostAsync($"/api/v1/shipments/{shipmentId}/customs-clearance",
            Json(new { customsCaseId, customsCaseNumber = "CUS-IT-2026-0001" }));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var shipment = JsonSerializer.Deserialize<JsonElement>(
            await (await _client.GetAsync($"/api/v1/shipments/{shipmentId}")).Content.ReadAsStreamAsync());

        var customs = shipment.GetProperty("customs");
        Assert.True(customs.GetProperty("isCustomsCleared").GetBoolean());
        Assert.Equal("CUS-IT-2026-0001", customs.GetProperty("customsCaseNumber").GetString());
        Assert.Equal(customsCaseId, customs.GetProperty("customsCaseId").GetGuid());
        Assert.Equal("CustomsCleared", shipment.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ConfirmCustomsClearance_WithoutPermission_ReturnsForbidden()
    {
        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", Token(TenantA, UserA, "shipping.create", "shipping.read"));

        var createResponse = await _client.PostAsync("/api/v1/shipments", CreateShipmentBody("ACME-IT-012"));
        var created = JsonSerializer.Deserialize<JsonElement>(
            await createResponse.Content.ReadAsStreamAsync());
        var shipmentId = created.GetProperty("shipmentId").GetGuid();

        var response = await _client.PostAsync($"/api/v1/shipments/{shipmentId}/customs-clearance",
            Json(new { customsCaseId = Guid.NewGuid(), customsCaseNumber = "CUS-IT-2026-0002" }));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
