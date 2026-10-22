using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Shared.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<ContainerFixture>
{
    protected readonly ContainerFixture ContainerFixture;
    protected readonly HttpClient Client = null!;
    protected readonly WebApplicationFactory<object> Factory = null!; // Simplified for base

    protected BaseIntegrationTest(ContainerFixture fixture)
    {
        ContainerFixture = fixture;
        // The actual factory will be provided by the service-specific implementation
    }
}
