using System;
using MediatR;
using Ectn.Domain.Enums;

namespace Ectn.Application.Commands;

public class CreateEctnCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public string ApplicationReference { get; set; }
    public EctnType Type { get; set; }
    public Guid CreatedBy { get; set; }
}

public class CreateEctnCommandHandler : IRequestHandler<CreateEctnCommand, Guid>
{
    // Normally injected via DI: IEctnRepository, IUnitOfWork
    
    public async Task<Guid> Handle(CreateEctnCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate (FluentValidation would intercept before this point)
        
        // 2. Map command to Domain Entity
        var ectn = new Domain.Entities.Ectn(
            request.TenantId,
            request.ApplicationReference,
            request.Type,
            request.CreatedBy
        );

        // 3. Save via Repository
        // await _repository.AddAsync(ectn, cancellationToken);
        // await _unitOfWork.CommitAsync(cancellationToken);

        // 4. Return the new ID
        return ectn.Id;
    }
}
