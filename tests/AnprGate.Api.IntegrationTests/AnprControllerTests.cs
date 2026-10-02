using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AnprGate.Api.IntegrationTests;

public class AnprControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnprControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostIngest_WithoutAuth_ShouldReturn_Unauthorized()
    {
        // Arrange: HttpClient macmal ah oo aan wadan wax Token ah
        var client = _factory.CreateClient();
        var content = new StringContent("{\"plateNumber\": \"SO-1234\"}", Encoding.UTF8, "application/json");

        // Act: Diritaanka POST request
        var response = await client.PostAsync("/api/v1/anpr/ingest", content);

        // Assert: Qalabka ANPR waa inuu wataa Token, haddii kale API-gu waa inuu diidaa (401)
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetIngest_ShouldReturn_MethodNotAllowed_Or_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act: Diritaanka GET request (halkii POST laga rabo)
        var response = await client.GetAsync("/api/v1/anpr/ingest");

        // Assert: Endpoint-kani POST kaliya ayuu qaataa
        Assert.True(response.StatusCode == HttpStatusCode.MethodNotAllowed || 
                    response.StatusCode == HttpStatusCode.NotFound || 
                    response.StatusCode == HttpStatusCode.Unauthorized);
    }
}
