using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Customs.Domain.Entities;
using FluentValidation;

namespace Customs.Application.Commands;

public class ReceiveDeclarationCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public string DeclarationExternalId { get; set; }
    public string DeclarationNumber { get; set; }
    public string ECTNExternalId { get; set; }
    public string ECTNNumber { get; set; }
    public string ProcedureCode { get; set; }
    public string ImporterReference { get; set; }
    public string ExporterReference { get; set; }
    public string DeclarantReference { get; set; }
    public string PortOfEntry { get; set; }
    public string PortOfExit { get; set; }
    public string CountryOfOrigin { get; set; }
    public string CountryOfDestination { get; set; }
    public Guid UserId { get; set; }
}

public class ReceiveDeclarationCommandValidator : AbstractValidator<ReceiveDeclarationCommand>
{
    public ReceiveDeclarationCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.DeclarationExternalId).NotEmpty();
        RuleFor(x => x.DeclarationNumber).NotEmpty();
        RuleFor(x => x.ProcedureCode).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public interface ICustomsCaseNumberGenerator
{
    string Generate(Guid tenantId);
}

public interface ICustomsDbContext
{
    // DbSet for Entities (To be implemented by EF Core DbContext)
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void AddCustomsCase(CustomsCase customsCase);
}

public class ReceiveDeclarationCommandHandler : IRequestHandler<ReceiveDeclarationCommand, Guid>
{
    private readonly ICustomsDbContext _context;
    private readonly ICustomsCaseNumberGenerator _numberGenerator;

    public ReceiveDeclarationCommandHandler(ICustomsDbContext context, ICustomsCaseNumberGenerator numberGenerator)
    {
        _context = context;
        _numberGenerator = numberGenerator;
    }

    public async Task<Guid> Handle(ReceiveDeclarationCommand request, CancellationToken cancellationToken)
    {
        var caseNumber = _numberGenerator.Generate(request.TenantId);

        var customsCase = new CustomsCase(
            request.TenantId,
            request.DeclarationExternalId,
            request.DeclarationNumber,
            request.ECTNExternalId,
            request.ECTNNumber,
            caseNumber,
            request.ProcedureCode,
            request.ImporterReference,
            request.ExporterReference,
            request.DeclarantReference,
            request.PortOfEntry,
            request.PortOfExit,
            request.CountryOfOrigin,
            request.CountryOfDestination,
            request.UserId
        );

        _context.AddCustomsCase(customsCase);
        
        // Outbox event could be published here
        
        await _context.SaveChangesAsync(cancellationToken);

        return customsCase.Id;
    }
}
