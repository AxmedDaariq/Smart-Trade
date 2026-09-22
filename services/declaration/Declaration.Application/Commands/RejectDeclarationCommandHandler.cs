using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Declaration.Application.Interfaces;
using Declaration.Domain.Enums;

namespace Declaration.Application.Commands;

public class RejectDeclarationCommandHandler : IRequestHandler<RejectDeclarationCommand, bool>
{
    private readonly ICustomsDeclarationRepository _repository;

    public RejectDeclarationCommandHandler(ICustomsDeclarationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(RejectDeclarationCommand request, CancellationToken cancellationToken)
    {
        var declaration = await _repository.GetByIdAndTenantAsync(request.DeclarationId, request.TenantId);
        
        if (declaration == null)
            throw new Exception("DECLARATION_NOT_FOUND");

        if (string.IsNullOrWhiteSpace(request.ReasonCode) || string.IsNullOrWhiteSpace(request.Reason))
            throw new Exception("DECLARATION_REJECTION_REASON_REQUIRED");

        // Assuming a Reject method exists in the aggregate or we manually update it
        // declaration.Reject(request.ReasonCode, request.Reason, request.UserId);
        
        await _repository.UpdateAsync(declaration);
        return true;
    }
}
