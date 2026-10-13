using System;
using System.Threading.Tasks;
using Ectn.Domain.Entities;

namespace Ectn.Domain.Repositories;

public interface IEctnRepository
{
    Task<Ectn> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(Ectn ectn);
    Task UpdateAsync(Ectn ectn);
    Task SaveChangesAsync();
}
