using System;
using System.Threading.Tasks;
using Customs.Domain.Entities;
using Customs.Infrastructure.Persistence;
using Customs.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Customs.UnitTests.Repositories;

/// <summary>
/// Unit tests for <see cref="CustomsRepository"/> using in-memory <see cref="CustomsDbContext"/>.
/// </summary>
public sealed class CustomsRepositoryTests : IDisposable
{
    private readonly CustomsDbContext _dbContext;
    private readonly CustomsRepository _repository;

    public CustomsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CustomsDbContext>()
            .UseInMemoryDatabase(databaseName: $"CustomsRepoTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new CustomsDbContext(options);
        _repository = new CustomsRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    private static CustomsCase CreateSampleCase(Guid tenantId) => new(
        tenantId: tenantId,
        declarationExternalId: "DEC-EXT-REPO-01",
        declarationNumber: "1000999",
        ectnExternalId: "ECTN-REPO-01",
        ectnNumber: "ECTN-2026-REPO",
        caseNumber: "CC-2026-REPO-001",
        procedureCode: "IM4",
        importerReference: "IMP-01",
        exporterReference: "EXP-01",
        declarantReference: "DCL-01",
        portOfEntry: "BERBERA",
        portOfExit: "JEBEL_ALI",
        countryOfOrigin: "AE",
        countryOfDestination: "SO",
        userId: Guid.NewGuid()
    );

    [Fact]
    public async Task AddAsync_And_SaveChangesAsync_PersistsCaseToDatabase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var customsCase = CreateSampleCase(tenantId);

        // Act
        await _repository.AddAsync(customsCase);
        await _repository.SaveChangesAsync();

        // Assert
        var persisted = await _dbContext.CustomsCases.FirstOrDefaultAsync(c => c.Id == customsCase.Id);
        persisted.Should().NotBeNull();
        persisted!.CaseNumber.Should().Be("CC-2026-REPO-001");
        persisted.TenantId.Should().Be(tenantId);
        persisted.Status.Should().Be("Received");
    }

    [Fact]
    public async Task GetCaseByIdAsync_MatchingIdAndTenant_ReturnsCustomsCase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var customsCase = CreateSampleCase(tenantId);
        await _dbContext.CustomsCases.AddAsync(customsCase);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCaseByIdAsync(customsCase.Id, tenantId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customsCase.Id);
        result.DeclarationNumber.Should().Be("1000999");
    }

    [Fact]
    public async Task GetCaseByIdAsync_DifferentTenantId_ReturnsNull_EnforcingTenantIsolation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();
        var customsCase = CreateSampleCase(tenantId);
        await _dbContext.CustomsCases.AddAsync(customsCase);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCaseByIdAsync(customsCase.Id, otherTenantId);

        // Assert: Cross-tenant access must be denied (null returned)
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCaseByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var result = await _repository.GetCaseByIdAsync(Guid.NewGuid(), tenantId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesCaseAndPersistsChanges()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var customsCase = CreateSampleCase(tenantId);
        await _dbContext.CustomsCases.AddAsync(customsCase);
        await _dbContext.SaveChangesAsync();

        // Act
        customsCase.Status = "RiskAssessment";
        await _repository.UpdateAsync(customsCase);
        await _repository.SaveChangesAsync();

        // Assert
        var updated = await _dbContext.CustomsCases.FirstOrDefaultAsync(c => c.Id == customsCase.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be("RiskAssessment");
    }
}
