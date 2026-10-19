using Customs.Application.Commands;
using Customs.Infrastructure.Persistence;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ReceiveDeclarationCommand).Assembly));

// A05/A07: the previous AddJwtBearer() call had no signing key, issuer or audience configured, so
// it could neither authenticate nor validate anything. JWT validation is now shared with the other
// services through the platform building block (32-byte HS256 key, issuer/audience/lifetime checks).
builder.Services.AddPlatformJwtAuthentication(builder.Configuration);

// Platform-wide security defaults: deny-by-default fallback policy (A01), rate limiting (A07),
// config-driven CORS, ProblemDetails exception handling and security headers (A05/A09).
builder.Services.AddPlatformSecurity(builder.Configuration);

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("customs.case.create", p => p.RequireClaim("permissions", "customs.case.create"));
    opts.AddPolicy("customs.case.read",   p => p.RequireClaim("permissions", "customs.case.read"));
    opts.AddPolicy("customs.risk.assess", p => p.RequireClaim("permissions", "customs.risk.assess"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<CustomsDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddScoped<ICustomsDbContext>(sp => sp.GetRequiredService<CustomsDbContext>());
}

builder.Services.AddScoped<ICustomsCaseNumberGenerator, Customs.Infrastructure.Services.CustomsCaseNumberGenerator>();
builder.Services.AddScoped<Customs.Domain.Repositories.ICustomsRepository, Customs.Infrastructure.Repositories.CustomsRepository>();

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
        var db = sp.GetService<CustomsDbContext>();
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
