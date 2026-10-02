using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Fleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Application.Commands;

public class CreateDispatchCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public string? CargoReference { get; set; }
    public string? CustomsCaseReference { get; set; }
}

public interface IFleetDbContext
{
    DbSet<DispatchOrder> DispatchOrders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class CreateDispatchCommandHandler : IRequestHandler<CreateDispatchCommand, Guid>
{
    private readonly IFleetDbContext _context;

    public CreateDispatchCommandHandler(IFleetDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDispatchCommand request, CancellationToken cancellationToken)
    {
        var dispatchNumber = $"DSP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        
        var dispatch = new DispatchOrder
        {
            TenantId = request.TenantId,
            FleetCompanyId = request.FleetCompanyId,
            VehicleId = request.VehicleId,
            DriverId = request.DriverId,
            DispatchReference = dispatchNumber,
            CargoReference = request.CargoReference,
            CustomsReleaseReference = request.CustomsCaseReference,
            Status = "Draft",
            Priority = "Normal"
        };

        _context.DispatchOrders.Add(dispatch);
        
        await _context.SaveChangesAsync(cancellationToken);

        return dispatch.Id;
    }
}
