using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Declaration.Application.Interfaces;
using Declaration.Domain.Enums;

namespace Declaration.Application.Commands;

public class AcceptDeclarationCommandHandler : IRequestHandler<AcceptDeclarationCommand, bool>
{
    private readonly ICustomsDeclarationRepository _repository;

    public AcceptDeclarationCommandHandler(ICustomsDeclarationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(AcceptDeclarationCommand request, CancellationToken cancellationToken)
    {
        var declaration = await _repository.GetByIdAndTenantAsync(request.DeclarationId, request.TenantId);
        
        if (declaration == null)
            throw new Exception("DECLARATION_NOT_FOUND");

        declaration.Accept(request.DeclarationNumber, request.UserId);
        
        await _repository.UpdateAsync(declaration);
        return true;
    }
}
