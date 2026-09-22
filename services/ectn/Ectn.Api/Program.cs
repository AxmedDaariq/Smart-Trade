using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ectn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Ectn.Application.Commands.CreateEctnCommand).Assembly));

// Configure Database
builder.Services.AddDbContext<EctnDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Authentication (Symmetric Key matches Phase 02 for dev, Vault for prod)
var secretKey = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is missing from production configuration.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// Configure Permission-Based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ectn.create", policy => policy.RequireClaim("permissions", "ectn.create"));
    options.AddPolicy("ectn.read", policy => policy.RequireClaim("permissions", "ectn.read"));
    options.AddPolicy("ectn.submit", policy => policy.RequireClaim("permissions", "ectn.submit"));
    options.AddPolicy("ectn.approve", policy => policy.RequireClaim("permissions", "ectn.approve"));
    options.AddPolicy("ectn.issue", policy => policy.RequireClaim("permissions", "ectn.issue"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
