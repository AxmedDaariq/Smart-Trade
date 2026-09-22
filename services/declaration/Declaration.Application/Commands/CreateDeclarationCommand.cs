using System;
using MediatR;
using Declaration.Domain.Enums;

namespace Declaration.Application.Commands;

public class CreateDeclarationCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public string ApplicationReference { get; set; }
    public DeclarationType DeclarationType { get; set; }
    public string ProcedureCode { get; set; }
    public Guid CreatedBy { get; set; }
}
