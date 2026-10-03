using System;
using System.Threading;
using System.Threading.Tasks;
using Declaration.Application.Commands;
using Declaration.Application.Interfaces;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;
using Moq;

namespace Declaration.Tests.Application.Commands;

public class CreateDeclarationCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsDraftDeclarationAndReturnsId()
    {
        var repository = new Mock<ICustomsDeclarationRepository>();
        repository
            .Setup(r => r.AddAsync(It.IsAny<CustomsDeclaration>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateDeclarationCommandHandler(repository.Object);
        var command = new CreateDeclarationCommand
        {
            TenantId = Guid.NewGuid(),
            ApplicationReference = "APP-REF-1",
            DeclarationType = DeclarationType.Import,
            ProcedureCode = "PROC-1",
            CreatedBy = Guid.NewGuid()
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
        repository.Verify(
            r => r.AddAsync(It.Is<CustomsDeclaration>(d =>
                d.Id == result &&
                d.TenantId == command.TenantId &&
                d.ApplicationReference == command.ApplicationReference &&
                d.ProcedureCode == command.ProcedureCode &&
                d.CreatedBy == command.CreatedBy &&
                d.Status == DeclarationStatus.Draft.ToString())),
            Times.Once);
    }
}
