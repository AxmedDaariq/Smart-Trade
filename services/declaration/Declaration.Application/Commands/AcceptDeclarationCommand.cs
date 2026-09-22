using System;
using MediatR;

namespace Declaration.Application.Commands;

public class AcceptDeclarationCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public Guid DeclarationId { get; set; }
    public Guid UserId { get; set; }
    public string DeclarationNumber { get; set; }
}
