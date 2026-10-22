using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Web;
using Ectn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Ectn.Application.Commands.CreateEctnCommand).Assembly));

// Configure Database
builder.Services.AddDbContext<EctnDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

// Platform-wide security defaults: deny-by-default fallback policy (A01), rate limiting (A07),
// config-driven CORS, ProblemDetails exception handling and security headers (A05/A09).
builder.Services.AddPlatformSecurity(builder.Configuration);

// Configure Permission-Based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ectn.create", policy => policy.RequireClaim("permissions", "ectn.create"));
    options.AddPolicy("ectn.read", policy => policy.RequireClaim("permissions", "ectn.read"));
    options.AddPolicy("ectn.submit", policy => policy.RequireClaim("permissions", "ectn.submit"));
    options.AddPolicy("ectn.approve", policy => policy.RequireClaim("permissions", "ectn.approve"));
    options.AddPolicy("ectn.issue", policy => policy.RequireClaim("permissions", "ectn.issue"));
    options.AddPolicy("ectn.statistics.read", policy => policy.RequireClaim("permissions", "ectn.statistics.read"));
    options.AddPolicy("ectn.documents.upload", policy => policy.RequireClaim("permissions", "ectn.documents.upload"));
    options.AddPolicy("ectn.documents.read", policy => policy.RequireClaim("permissions", "ectn.documents.read"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.MapGet("/health/ready", async (EctnDbContext db, CancellationToken ct) =>
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
