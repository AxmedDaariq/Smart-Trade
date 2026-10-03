using System;
using System.Threading;
using System.Threading.Tasks;
using Declaration.Application.Commands;
using Declaration.Application.Interfaces;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;
using Moq;

namespace Declaration.Tests.Application.Commands;

public class AcceptDeclarationCommandHandlerTests
{
    private readonly Mock<ICustomsDeclarationRepository> _repository = new();

    private static CustomsDeclaration NewDeclaration(Guid tenantId) =>
        new(tenantId, "APP-REF-1", DeclarationType.Import, "PROC-1", Guid.NewGuid());

    [Fact]
    public async Task Handle_AcceptsDeclarationAndPersists()
    {
        var tenantId = Guid.NewGuid();
        var declaration = NewDeclaration(tenantId);
        _repository.Setup(r => r.GetByIdAndTenantAsync(declaration.Id, tenantId)).ReturnsAsync(declaration);

        var command = new AcceptDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationId = declaration.Id,
            UserId = Guid.NewGuid(),
            DeclarationNumber = "DEC-2026-0001"
        };

        var handler = new AcceptDeclarationCommandHandler(_repository.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(DeclarationStatus.Accepted.ToString(), declaration.Status);
        Assert.Equal("DEC-2026-0001", declaration.DeclarationNumber);
        _repository.Verify(r => r.UpdateAsync(declaration), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDeclarationNotFound_Throws()
    {
        var tenantId = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByIdAndTenantAsync(It.IsAny<Guid>(), tenantId))
            .ReturnsAsync((CustomsDeclaration)null!);

        var command = new AcceptDeclarationCommand
        {
            TenantId = tenantId,
            DeclarationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DeclarationNumber = "DEC-2026-0002"
        };

        var handler = new AcceptDeclarationCommandHandler(_repository.Object);

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}
