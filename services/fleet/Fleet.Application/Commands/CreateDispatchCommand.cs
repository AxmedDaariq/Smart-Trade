using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Fleet.Domain.Entities;
using Fleet.Infrastructure.Persistence;

namespace Fleet.Application.Commands;

public class CreateDispatchCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public string CargoReference { get; set; }
    public string CustomsCaseReference { get; set; }
}

public class CreateDispatchCommandHandler : IRequestHandler<CreateDispatchCommand, Guid>
{
    private readonly FleetDbContext _context;

    public CreateDispatchCommandHandler(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDispatchCommand request, CancellationToken cancellationToken)
    {
        var dispatchNumber = $"DSP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        
        var dispatch = new DispatchOrder(
            request.TenantId,
            dispatchNumber,
            request.FleetCompanyId,
            request.VehicleId,
            request.DriverId,
            request.CargoReference,
            request.CustomsCaseReference
        );

        _context.DispatchOrders.Add(dispatch);
        
        await _context.SaveChangesAsync(cancellationToken);

        return dispatch.Id;
    }
}
