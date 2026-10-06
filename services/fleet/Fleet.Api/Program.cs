using BuildingBlocks.Web;
using Fleet.Application.Commands;
using Fleet.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDispatchCommand).Assembly));

// A01 (critical): this service previously mapped controllers without any authentication or
// authorization middleware, so every [Authorize] attribute on DispatchesController was silently
// ignored and the dispatch API was reachable anonymously. JWT validation, the deny-by-default
// fallback policy, rate limiting and ProblemDetails are now wired in.
builder.Services.AddPlatformJwtAuthentication(builder.Configuration);
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("fleet.dispatch.create", policy => policy.RequireClaim("permissions", "fleet.dispatch.create"));
    options.AddPolicy("fleet.dispatch.read", policy => policy.RequireClaim("permissions", "fleet.dispatch.read"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<FleetDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddScoped<IFleetDbContext>(sp => sp.GetRequiredService<FleetDbContext>());
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

app.Run();

public partial class Program { }
