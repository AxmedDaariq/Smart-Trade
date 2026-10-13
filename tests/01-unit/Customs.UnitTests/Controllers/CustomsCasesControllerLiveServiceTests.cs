using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Customs.Api.Controllers;
using Customs.Application.Commands;
using Customs.Infrastructure.Persistence;
using Customs.Infrastructure.Services;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Customs.UnitTests.Controllers;

/// <summary>
/// Direct Integration Test:
/// Xidhiidh toos ah oo ka dhexeeya CustomsCasesController dhabta ah iyo Services/Handlers dhabta ah
/// (ReceiveDeclarationCommandHandler, CustomsCaseNumberGenerator, iyo CustomsDbContext dhab ah oo in-memory ah)
/// iyada oo aan la isticmaalin mock services.
/// </summary>
public sealed class CustomsCasesControllerLiveServiceTests : IDisposable
{
    private readonly CustomsDbContext _dbContext;
    private readonly CustomsCasesController _controller;
    private readonly ServiceProvider _serviceProvider;

    public CustomsCasesControllerLiveServiceTests()
    {
        // 1. Setup in-memory Real Database
        var options = new DbContextOptionsBuilder<CustomsDbContext>()
            .UseInMemoryDatabase(databaseName: $"CustomsLiveTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new CustomsDbContext(options);

        // 2. Setup Real Services in DI Container
        var services = new ServiceCollection();
        services.AddSingleton<ICustomsDbContext>(_dbContext);
        services.AddSingleton<ICustomsCaseNumberGenerator, CustomsCaseNumberGenerator>();

        // Register Real MediatR with the application assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ReceiveDeclarationCommand).Assembly));

        _serviceProvider = services.BuildServiceProvider();
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        // 3. Inject REAL mediator into the REAL CustomsCasesController
        _controller = new CustomsCasesController(mediator);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _serviceProvider.Dispose();
    }

    [Fact]
    public async Task CreateCase_WithRealControllerAndRealService_ShouldPersistEntityInDatabase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("sub", userId.ToString())
        }, "TestAuthentication");
        httpContext.User = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var command = new ReceiveDeclarationCommand
        {
            DeclarationExternalId = "DEC-EXT-9988",
            DeclarationNumber = "DECL-2026-BERBERA-001",
            ECTNExternalId = "ECTN-9988",
            ECTNNumber = "ECTN-2026-BERBERA-001",
            ProcedureCode = "IM4",
            ImporterReference = "IMP-SOMALIA-01",
            ExporterReference = "EXP-DUBAI-01",
            DeclarantReference = "DEC-AGENT-01",
            PortOfEntry = "BERBERA",
            PortOfExit = "JEBEL_ALI",
            CountryOfOrigin = "AE",
            CountryOfDestination = "SO"
        };

        // Act: Controller-ka toos ayaan u wacaynaa -> wuxuuna wacayaa Real Mediator -> Real Handler -> Real DbContext
        var actionResult = await _controller.CreateCase(command);

        // Assert 1: Controller response
        var createdResult = actionResult.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.RouteValues.Should().ContainKey("id");

        var createdId = (Guid)createdResult.RouteValues!["id"]!;
        createdId.Should().NotBeEmpty();

        // Assert 2: Database state - entity must physically exist in in-memory database
        var persistedCase = await _dbContext.CustomsCases.FirstOrDefaultAsync(c => c.Id == createdId);
        persistedCase.Should().NotBeNull();
        persistedCase!.TenantId.Should().Be(tenantId);
        persistedCase.CreatedBy.Should().Be(userId);
        persistedCase.DeclarationNumber.Should().Be("DECL-2026-BERBERA-001");
        persistedCase.ProcedureCode.Should().Be("IM4");
        persistedCase.Status.Should().Be("Received");
        persistedCase.CaseNumber.Should().StartWith("CC-");
        persistedCase.PortOfEntry.Should().Be("BERBERA");
        persistedCase.CountryOfDestination.Should().Be("SO");
        persistedCase.OpenedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateCase_WithRealController_ShouldForbid_WhenTenantClaimIsMissing()
    {
        // Arrange: Missing tenant_id claim
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var command = new ReceiveDeclarationCommand
        {
            DeclarationNumber = "DECL-TEST-002"
        };

        // Act
        var actionResult = await _controller.CreateCase(command);

        // Assert
        actionResult.Should().BeOfType<ForbidResult>();

        // Hubi in waxba aan lagu darin Database-ka
        var count = await _dbContext.CustomsCases.CountAsync();
        count.Should().Be(0);
    }
}
