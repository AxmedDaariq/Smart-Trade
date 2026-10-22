using System;
using System.Threading;
using System.Threading.Tasks;
using Customs.Application.Commands;
using Customs.Domain.Entities;
using Moq;
using Xunit;

namespace Customs.Tests.Application.Commands;

public class ReceiveDeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDbContext> _mockContext;
    private readonly Mock<ICustomsCaseNumberGenerator> _mockNumberGenerator;
    private readonly ReceiveDeclarationCommandHandler _handler;

    public ReceiveDeclarationCommandHandlerTests()
    {
        _mockContext = new Mock<ICustomsDbContext>();
        _mockNumberGenerator = new Mock<ICustomsCaseNumberGenerator>();
        _handler = new ReceiveDeclarationCommandHandler(_mockContext.Object, _mockNumberGenerator.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_CustomsCase_And_Return_Id()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var generatedCaseNumber = "CASE-2026-001";

        _mockNumberGenerator
            .Setup(g => g.Generate(tenantId))
            .Returns(generatedCaseNumber);

        _mockContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ReceiveDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationExternalId = "DEC-EXT-001",
            DeclarationNumber = "1000001",
            ECTNExternalId = "ECTN-EXT-001",
            ECTNNumber = "ECTN-2026-GEN-001",
            ProcedureCode = "IM4",
            ImporterReference = "IMP-001",
            ExporterReference = "EXP-002",
            DeclarantReference = "DCL-003",
            PortOfEntry = "GEN",
            PortOfExit = "SHA",
            CountryOfOrigin = "CN",
            CountryOfDestination = "SO",
            UserId = userId
        };

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);

        // Verify AddCustomsCase was called once
        _mockContext.Verify(c => c.AddCustomsCase(It.Is<CustomsCase>(cc =>
            cc.TenantId == tenantId &&
            cc.DeclarationNumber == "1000001" &&
            cc.CaseNumber == generatedCaseNumber &&
            cc.Status == "Received" &&
            cc.CreatedBy == userId
        )), Times.Once);

        // Verify SaveChangesAsync was called once
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify case number generator was called
        _mockNumberGenerator.Verify(g => g.Generate(tenantId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Use_Generated_CaseNumber()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var expectedCaseNumber = "CASE-2026-XYZ-999";

        _mockNumberGenerator
            .Setup(g => g.Generate(tenantId))
            .Returns(expectedCaseNumber);

        _mockContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new ReceiveDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationExternalId = "DEC-EXT-002",
            DeclarationNumber = "2000002",
            ProcedureCode = "IM4",
            UserId = Guid.NewGuid()
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert: case number came from generator
        _mockContext.Verify(c => c.AddCustomsCase(It.Is<CustomsCase>(cc =>
            cc.CaseNumber == expectedCaseNumber
        )), Times.Once);
    }
}
