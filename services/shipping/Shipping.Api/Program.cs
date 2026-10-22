using Shipping.Application.Commands;
using Shipping.Infrastructure.Persistence;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ──────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

// ── MVC / API ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Smart Trade — Shipping API", Version = "v1" });
});

// ── MediatR (CQRS) ───────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateShipmentCommand).Assembly));

// ── Database ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ShippingDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("ShippingDb")));

builder.Services.AddScoped<IShippingDbContext>(sp => sp.GetRequiredService<ShippingDbContext>());
builder.Services.AddScoped<IShippingQueryContext>(sp => sp.GetRequiredService<ShippingDbContext>());
builder.Services.AddScoped<IShipmentNumberGenerator, ShipmentNumberGenerator>();

// ── Auth & Security ───────────────────────────────────────────────────────────
// A05/A07: shared JWT validation through platform building block
// (32-byte HS256 key, issuer/audience/lifetime checks).
builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

// Platform-wide security defaults: deny-by-default fallback policy (A01),
// rate limiting (A07), config-driven CORS, ProblemDetails and security headers (A05/A09).
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("shipping.create",          p => p.RequireClaim("permissions", "shipping.create"));
    opts.AddPolicy("shipping.read",            p => p.RequireClaim("permissions", "shipping.read"));
    opts.AddPolicy("shipping.status.update",   p => p.RequireClaim("permissions", "shipping.status.update"));
    opts.AddPolicy("shipping.tracking.record", p => p.RequireClaim("permissions", "shipping.tracking.record"));
    opts.AddPolicy("shipping.customs.confirm", p => p.RequireClaim("permissions", "shipping.customs.confirm"));
});

// ── Build ─────────────────────────────────────────────────────────────────────
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
app.MapGet("/health/ready", async (ShippingDbContext db, CancellationToken ct) =>
{
    try
    {
        if (await db.Database.CanConnectAsync(ct)) return (IResult)Results.Ok(new { status = "ready" });
        return Results.Problem(statusCode: Microsoft.AspNetCore.Http.StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
    catch
    {
        return Results.Problem(statusCode: Microsoft.AspNetCore.Http.StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
}).AllowAnonymous();

app.Run();

// Integration tests use WebApplicationFactory<Program>, which requires the
// entry-point class to be public (same pattern as the other API services).
public partial class Program { }
