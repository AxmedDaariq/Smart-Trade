using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Declaration.Application.Interfaces;
using Declaration.Domain.Entities;

namespace Declaration.Application.Commands;

public class CreateDeclarationCommandHandler : IRequestHandler<CreateDeclarationCommand, Guid>
{
    private readonly ICustomsDeclarationRepository _repository;

    public CreateDeclarationCommandHandler(ICustomsDeclarationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateDeclarationCommand request, CancellationToken cancellationToken)
    {
        var declaration = new CustomsDeclaration(
            request.TenantId,
            request.ApplicationReference,
            request.DeclarationType,
            request.ProcedureCode,
            request.CreatedBy
        );

        await _repository.AddAsync(declaration);
        return declaration.Id;
    }
}
