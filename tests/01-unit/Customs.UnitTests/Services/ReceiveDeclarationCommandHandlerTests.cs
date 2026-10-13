using System;
using System.Threading;
using System.Threading.Tasks;
using Customs.Application.Commands;
using Customs.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Customs.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="ReceiveDeclarationCommandHandler"/>.
/// </summary>
public sealed class ReceiveDeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDbContext> _contextMock = new();
    private readonly Mock<ICustomsCaseNumberGenerator> _numberGeneratorMock = new();
    private readonly ReceiveDeclarationCommandHandler _handler;

    public ReceiveDeclarationCommandHandlerTests()
    {
        _handler = new ReceiveDeclarationCommandHandler(_contextMock.Object, _numberGeneratorMock.Object);
    }

    private static ReceiveDeclarationCommand CreateSampleCommand(Guid tenantId, Guid userId) => new()
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

    [Fact]
    public async Task Handle_ValidCommand_CreatesCaseAndReturnsCaseId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var generatedCaseNumber = "CASE-2026-SOM-001";

        _numberGeneratorMock
            .Setup(g => g.Generate(tenantId))
            .Returns(generatedCaseNumber);

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        CustomsCase? capturedCase = null;
        _contextMock
            .Setup(c => c.AddCustomsCase(It.IsAny<CustomsCase>()))
            .Callback<CustomsCase>(cc => capturedCase = cc);

        var command = CreateSampleCommand(tenantId, userId);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultId.Should().NotBeEmpty();
        capturedCase.Should().NotBeNull();
        capturedCase!.Id.Should().Be(resultId);
        capturedCase.TenantId.Should().Be(tenantId);
        capturedCase.CreatedBy.Should().Be(userId);
        capturedCase.CaseNumber.Should().Be(generatedCaseNumber);
        capturedCase.DeclarationNumber.Should().Be("1000001");
        capturedCase.ProcedureCode.Should().Be("IM4");
        capturedCase.Status.Should().Be("Received");
        capturedCase.PortOfEntry.Should().Be("GEN");
        capturedCase.CountryOfOrigin.Should().Be("CN");
        capturedCase.CountryOfDestination.Should().Be("SO");

        _contextMock.Verify(c => c.AddCustomsCase(It.IsAny<CustomsCase>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _numberGeneratorMock.Verify(g => g.Generate(tenantId), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDatabaseSaveFails_PropagatesException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _numberGeneratorMock
            .Setup(g => g.Generate(tenantId))
            .Returns("CC-FAIL-001");

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection timed out"));

        var command = CreateSampleCommand(tenantId, userId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database connection timed out");
    }

    [Fact]
    public async Task Handle_SetsAllDeclarationReferencesCorrectly()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _numberGeneratorMock
            .Setup(g => g.Generate(tenantId))
            .Returns("CC-REFS-001");

        CustomsCase? capturedCase = null;
        _contextMock
            .Setup(c => c.AddCustomsCase(It.IsAny<CustomsCase>()))
            .Callback<CustomsCase>(cc => capturedCase = cc);

        var command = CreateSampleCommand(tenantId, userId);
        command.ECTNExternalId = "ECTN-EXT-XYZ";
        command.ECTNNumber = "ECTN-NUM-999";
        command.ImporterReference = "IMP-REF-77";
        command.ExporterReference = "EXP-REF-88";
        command.DeclarantReference = "DEC-REF-99";

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedCase.Should().NotBeNull();
        capturedCase!.ECTNExternalId.Should().Be("ECTN-EXT-XYZ");
        capturedCase.EctnNumber.Should().Be("ECTN-NUM-999");
        capturedCase.ImporterReference.Should().Be("IMP-REF-77");
        capturedCase.ExporterReference.Should().Be("EXP-REF-88");
        capturedCase.DeclarantReference.Should().Be("DEC-REF-99");
    }
}
