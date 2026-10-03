using AisVessel.Application.Commands;
using AisVessel.Application.Interfaces;
using AisVessel.Infrastructure.Persistence;
using AisVessel.Infrastructure.Repositories;
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

builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ais.berth.allocate", policy => policy.RequireClaim("permissions", "ais.berth.allocate"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AllocateBerthCommand).Assembly));

builder.Services.AddDbContext<AisDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IBerthAllocationRepository, BerthAllocationRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", async (AisDbContext db, CancellationToken ct) =>
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
});

app.Run();

public partial class Program { }
