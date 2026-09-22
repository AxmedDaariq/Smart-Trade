using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://identity-service"; 
        options.Audience = "declaration-api";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("declaration.create", policy => policy.RequireClaim("permissions", "declaration.create"));
    options.AddPolicy("declaration.read", policy => policy.RequireClaim("permissions", "declaration.read"));
    options.AddPolicy("declaration.submit", policy => policy.RequireClaim("permissions", "declaration.submit"));
    options.AddPolicy("declaration.review", policy => policy.RequireClaim("permissions", "declaration.review"));
    options.AddPolicy("declaration.accept", policy => policy.RequireClaim("permissions", "declaration.accept"));
    options.AddPolicy("declaration.reject", policy => policy.RequireClaim("permissions", "declaration.reject"));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDeclarationCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateDeclarationCommandValidator).Assembly);

builder.Services.AddDbContext<DeclarationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomsDeclarationRepository, CustomsDeclarationRepository>();
builder.Services.AddHttpClient<IEctnServiceClient, EctnServiceClient>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health/live", () => "Alive");
app.MapGet("/health/ready", () => "Ready");

app.Run();
