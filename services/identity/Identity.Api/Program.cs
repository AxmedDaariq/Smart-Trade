using Identity.Application.Interfaces;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Identity.Infrastructure.Services;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection(JwtTokenOptions.SectionName).Get<JwtTokenOptions>();
if (jwt == null)
{
    throw new InvalidOperationException("Jwt configuration is required.");
}
if (string.IsNullOrWhiteSpace(jwt.Secret) || Encoding.UTF8.GetByteCount(jwt.Secret) < 32 ||
    string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience) ||
    jwt.AccessTokenLifetimeMinutes is < 1 or > 60)
    throw new InvalidOperationException("Jwt configuration is invalid; provide a 32-byte secret, issuer, audience, and lifetime from 1 to 60 minutes.");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection(JwtTokenOptions.SectionName));
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IIdentityAuthService, IdentityAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

// Platform-wide security defaults: deny-by-default fallback policy (A01), rate limiting (A07),
// config-driven CORS, ProblemDetails exception handling and security headers (A05/A09).
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("identity.users.read", policy => policy.RequireClaim("permissions", "identity.users.read"));
    options.AddPolicy("identity.users.manage", policy => policy.RequireClaim("permissions", "identity.users.manage"));
    options.AddPolicy("identity.tenants.read", policy => policy.RequireClaim("permissions", "identity.tenants.read"));
    options.AddPolicy("identity.tenants.manage", policy => policy.RequireClaim("permissions", "identity.tenants.manage"));
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("identity-auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 30,
            Window = TimeSpan.FromMinutes(5),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

var app = builder.Build();
app.UsePlatformSecurity();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UsePlatformCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" })).AllowAnonymous();
app.MapGet("/health/ready", async (IdentityDbContext db, CancellationToken ct) =>
{
    try
    {
        if (await db.Database.CanConnectAsync(ct)) return (IResult)Results.Ok(new { status = "ready" });
        return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
    catch
    {
        return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
}).AllowAnonymous();

app.Run();

// Integration tests use WebApplicationFactory<Program>, which requires the
// entry-point class to be public (same pattern as the other API services).
public partial class Program { }
