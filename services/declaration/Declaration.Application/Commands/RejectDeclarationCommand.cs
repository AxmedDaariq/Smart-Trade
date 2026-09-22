using System;
using MediatR;

namespace Declaration.Application.Commands;

public class RejectDeclarationCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public Guid DeclarationId { get; set; }
    public Guid UserId { get; set; }
    public string ReasonCode { get; set; }
    public string Reason { get; set; }
}
