using AnprGate.Application.Commands;
using AnprGate.Infrastructure.Persistence;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IngestRecognitionCommand).Assembly));

// A01/A07: the pipeline used to call UseAuthentication/UseAuthorization without registering an
// authentication scheme or the "anpr.camera.machine" policy, so [Authorize] could never succeed.
// JWT validation and the deny-by-default fallback policy are now provided by the platform.
builder.Services.AddPlatformJwtAuthentication(builder.Configuration);
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("anpr.camera.machine", policy => policy.RequireClaim("permissions", "anpr.camera.machine"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AnprDbContext>(options => options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
}

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
app.MapGet("/health/ready", async (IServiceProvider sp, CancellationToken ct) =>
{
    try
    {
        var db = sp.GetService<AnprDbContext>();
        if (db != null && await db.Database.CanConnectAsync(ct)) return (IResult)Results.Ok(new { status = "ready" });
        return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
    catch
    {
        return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, title: "Database unavailable");
    }
}).AllowAnonymous();

app.Run();

public partial class Program { }
