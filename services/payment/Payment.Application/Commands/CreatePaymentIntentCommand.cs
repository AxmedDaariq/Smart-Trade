using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;

namespace Payment.Application.Commands;

public class CreatePaymentIntentCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceAggregateId { get; set; } = string.Empty;
    public Guid CustomsCaseId { get; set; }
    public string DeclarationNumber { get; set; } = string.Empty;
    public string ECTNNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? DueDate { get; set; }
}

public class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter code.");
        RuleFor(x => x.SourceSystem).NotEmpty().WithMessage("SourceSystem is required.");
    }
}

public interface IPaymentDbContext
{
    void AddPaymentIntent(PaymentIntent intent);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, Guid>
{
    private readonly IPaymentDbContext _context;

    public CreatePaymentIntentCommandHandler(IPaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var paymentReference = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        
        var money = new Money(request.Amount, request.Currency);

        var intent = new PaymentIntent(
            request.TenantId,
            paymentReference,
            request.SourceSystem,
            request.SourceAggregateId,
            request.CustomsCaseId,
            request.DeclarationNumber,
            request.ECTNNumber,
            money,
            request.DueDate
        );

        _context.AddPaymentIntent(intent);
        
        await _context.SaveChangesAsync(cancellationToken);

        return intent.Id;
    }
}
