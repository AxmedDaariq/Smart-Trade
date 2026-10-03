using Integration.Application.Commands;
using Integration.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Register the MediatR handlers that live in Integration.Application.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessWebhookCommand).Assembly));

// Persistence is only wired when a connection string is supplied
// (e.g. ConnectionStrings__DefaultConnection environment variable).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<IntegrationDbContext>(options => options.UseNpgsql(connectionString));
}

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.MapGet("/health/live", () => "Alive");

app.Run();

public partial class Program { }
