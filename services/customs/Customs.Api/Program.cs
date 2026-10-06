using Customs.Application.Commands;
using BuildingBlocks.Web;
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
