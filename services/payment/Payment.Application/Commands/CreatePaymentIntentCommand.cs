using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Payment.Domain.Entities;
using Payment.Domain.ValueObjects;
using Payment.Infrastructure.Persistence;

namespace Payment.Application.Commands;

public class CreatePaymentIntentCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public string ExternalReference { get; set; }
    public string SourceSystem { get; set; }
    public string SourceAggregateId { get; set; }
    public Guid CustomsCaseId { get; set; }
    public string DeclarationNumber { get; set; }
    public string ECTNNumber { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime? DueDate { get; set; }
}

public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, Guid>
{
    private readonly PaymentDbContext _context;

    public CreatePaymentIntentCommandHandler(PaymentDbContext context)
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

        _context.PaymentIntents.Add(intent);
        
        // Idempotency check and Outbox event logic would go here
        
        await _context.SaveChangesAsync(cancellationToken);

        return intent.Id;
    }
}
