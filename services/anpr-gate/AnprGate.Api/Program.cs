using AnprGate.Application.Commands;
using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
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
