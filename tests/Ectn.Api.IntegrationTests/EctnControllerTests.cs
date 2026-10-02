using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ectn.Api.IntegrationTests;

public class EctnControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EctnControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEctn_WithoutAuth_ShouldReturn_Unauthorized()
    {
        // Arrange: Waxaan abuuraynaa HTTP Client aan wadan wax Token ah (No Auth Token)
        var client = _factory.CreateClient();
        var fakeId = System.Guid.NewGuid();

        // Act: Waxaan isku dayaynaa in aan u dirno GET Request Endpoint-ka api/v1/ectn
        var response = await client.GetAsync($"/api/v1/ectn/{fakeId}");

        // Assert: Natiijadu waa in ay noqotaa 401 Unauthorized maadaama aynaan wadan Token
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
