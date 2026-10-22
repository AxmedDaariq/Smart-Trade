using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Web;
using MediatR;
using FluentValidation;
using Declaration.Application.Commands;
using Declaration.Application.Interfaces;
using Declaration.Infrastructure.Persistence;
using Declaration.Infrastructure.Repositories;
using Declaration.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

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
    options.AddPolicy("declaration.create", policy => policy.RequireClaim("permissions", "declaration.create"));
    options.AddPolicy("declaration.read", policy => policy.RequireClaim("permissions", "declaration.read"));
    options.AddPolicy("declaration.submit", policy => policy.RequireClaim("permissions", "declaration.submit"));
    options.AddPolicy("declaration.review", policy => policy.RequireClaim("permissions", "declaration.review"));
    options.AddPolicy("declaration.accept", policy => policy.RequireClaim("permissions", "declaration.accept"));
    options.AddPolicy("declaration.reject", policy => policy.RequireClaim("permissions", "declaration.reject"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDeclarationCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateDeclarationCommandValidator).Assembly);

builder.Services.AddDbContext<DeclarationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICustomsDeclarationRepository, CustomsDeclarationRepository>();
builder.Services.AddHttpClient<IEctnServiceClient, EctnServiceClient>();

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
app.MapGet("/health/ready", async (DeclarationDbContext db, CancellationToken ct) =>
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
