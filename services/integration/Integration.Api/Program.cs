using BuildingBlocks.Web;
using Integration.Api.Infrastructure;
using Integration.Application.Commands;
using Integration.Application.Services;
using Integration.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// A01/A07: the service previously ran with UseAuthorization() but no authentication scheme at all,
// so no endpoint could be authenticated and the [AllowAnonymous] webhook was the only policy in
// play. JWT validation, the deny-by-default fallback policy, rate limiting, config-driven CORS and
// ProblemDetails exception handling now come from the platform building block.
builder.Services.AddPlatformJwtAuthentication(builder.Configuration);
builder.Services.AddPlatformSecurity(builder.Configuration);
builder.Services.AddAuthorization();

// A08: inbound webhooks are authenticated with HMAC-SHA256 over the raw payload.
builder.Services.AddSingleton<IExternalRequestSigner, HmacExternalRequestSigner>();
builder.Services.AddSingleton<IWebhookSecretStore, ConfigurationWebhookSecretStore>();

// Register the MediatR handlers that live in Integration.Application.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessWebhookCommand).Assembly));

// Persistence is only wired when a connection string is supplied
// (e.g. ConnectionStrings__DefaultConnection environment variable).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<IntegrationDbContext>(options => options.UseNpgsql(connectionString));
}

var app = builder.Build();

app.UsePlatformSecurity();
app.UseHttpsRedirection();
app.UseRouting();
app.UsePlatformCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health/live", () => "Alive").AllowAnonymous();

app.Run();

public partial class Program { }
