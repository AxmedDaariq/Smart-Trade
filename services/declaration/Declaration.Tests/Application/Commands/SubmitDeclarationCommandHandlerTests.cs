using System;
using System.Threading;
using System.Threading.Tasks;
using Declaration.Application.Commands;
using Declaration.Application.Interfaces;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;
using Moq;

namespace Declaration.Tests.Application.Commands;

public class SubmitDeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDeclarationRepository> _repository = new();
    private readonly Mock<IEctnServiceClient> _ectnClient = new();

    private SubmitDeclarationCommandHandler CreateHandler() => new(_repository.Object, _ectnClient.Object);

    private static CustomsDeclaration NewDeclaration(Guid tenantId) =>
        new(tenantId, "APP-REF-1", DeclarationType.Import, "PROC-1", Guid.NewGuid());

    [Fact]
    public async Task Handle_WithValidEctn_SubmitsDeclaration()
    {
        var tenantId = Guid.NewGuid();
        var declaration = NewDeclaration(tenantId);
        _repository.Setup(r => r.GetByIdAndTenantAsync(declaration.Id, tenantId)).ReturnsAsync(declaration);
        _ectnClient.Setup(c => c.ValidateEctnAsync("ECTN-1", tenantId)).ReturnsAsync(true);

        var command = new SubmitDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationId = declaration.Id,
            UserId = Guid.NewGuid(),
            EctnReference = "ECTN-1"
        };

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(DeclarationStatus.Submitted.ToString(), declaration.Status);
        Assert.Equal("ECTN-1", declaration.EctnReference);
        _repository.Verify(r => r.UpdateAsync(declaration), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDeclarationNotFound_Throws()
    {
        var tenantId = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), tenantId))
            .ReturnsAsync((CustomsDeclaration)null!);

        var command = new SubmitDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            EctnReference = "ECTN-1"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenEctnInvalid_ThrowsAndDoesNotPersist()
    {
        var tenantId = Guid.NewGuid();
        var declaration = NewDeclaration(tenantId);
        _repository.Setup(r => r.GetByIdAndTenantAsync(declaration.Id, tenantId)).ReturnsAsync(declaration);
        _ectnClient.Setup(c => c.ValidateEctnAsync("BAD", tenantId)).ReturnsAsync(false);

        var command = new SubmitDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationId = declaration.Id,
            UserId = Guid.NewGuid(),
            EctnReference = "BAD"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateHandler().Handle(command, CancellationToken.None));

        Assert.Equal("DECLARATION_ECTN_INVALID", ex.Message);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<CustomsDeclaration>()), Times.Never);
    }
}
