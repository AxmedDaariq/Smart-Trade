using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Declaration.Application.Interfaces;

namespace Declaration.Application.Commands;

public class SubmitDeclarationCommandHandler : IRequestHandler<SubmitDeclarationCommand, bool>
{
    private readonly ICustomsDeclarationRepository _repository;
    private readonly IEctnServiceClient _ectnClient;

    public SubmitDeclarationCommandHandler(ICustomsDeclarationRepository repository, IEctnServiceClient ectnClient)
    {
        _repository = repository;
        _ectnClient = ectnClient;
    }

    public async Task<bool> Handle(SubmitDeclarationCommand request, CancellationToken cancellationToken)
    {
        var declaration = await _repository.GetByIdAndTenantAsync(request.DeclarationId, request.TenantId);
        
        if (declaration == null)
            throw new InvalidOperationException("DECLARATION_NOT_FOUND");

        // Validate ECTN
        var ectnValid = await _ectnClient.ValidateEctnAsync(request.EctnReference, request.TenantId);
        if (!ectnValid)
            throw new InvalidOperationException("DECLARATION_ECTN_INVALID");

        declaration.AssignEctnReference(request.EctnReference);
        declaration.Submit(request.UserId);

        await _repository.UpdateAsync(declaration);
        
        return true;
    }
}
