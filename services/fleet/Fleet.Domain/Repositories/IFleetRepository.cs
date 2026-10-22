using System;
using System.Threading.Tasks;
using Fleet.Domain.Entities;

namespace Fleet.Domain.Repositories;

public interface IFleetRepository
{
    Task<DispatchOrder> GetDispatchByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(DispatchOrder dispatch);
    Task UpdateAsync(DispatchOrder dispatch);
    Task SaveChangesAsync();
}
