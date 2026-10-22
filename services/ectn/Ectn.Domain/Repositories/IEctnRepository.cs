using System;
using System.Threading.Tasks;
using EctnEntity = Ectn.Domain.Entities.Ectn;

namespace Ectn.Domain.Repositories;

public interface IEctnRepository
{
    Task<EctnEntity?> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(EctnEntity ectn);
    Task UpdateAsync(EctnEntity ectn);
    Task SaveChangesAsync();
}
