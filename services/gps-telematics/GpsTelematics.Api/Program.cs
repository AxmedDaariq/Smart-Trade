using BuildingBlocks.Web;
using GpsTelematics.Application.Commands;
using GpsTelematics.Application.Interfaces;
using GpsTelematics.Infrastructure.Persistence;
using GpsTelematics.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

// Platform-wide security defaults: deny-by-default fallback policy (A01), rate limiting (A07),
// config-driven CORS, ProblemDetails exception handling and security headers (A05/A09).
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("gps.device.ingest", policy => policy.RequireClaim("permissions", "gps.device.ingest"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IngestTelemetryCommand).Assembly));

builder.Services.AddDbContext<GpsDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITelemetryIngestionRepository, TelemetryIngestionRepository>();

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

app.MapGet("/health/live", () => "Alive").AllowAnonymous();
app.MapGet("/health/ready", async (GpsDbContext db, CancellationToken ct) =>
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

public partial class Program { }
