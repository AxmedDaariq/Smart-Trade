using System;
using System.Threading.Tasks;
using Customs.Domain.Entities;

namespace Customs.Domain.Repositories;

public interface ICustomsRepository
{
    Task<CustomsCase> GetCaseByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(CustomsCase customsCase);
    Task UpdateAsync(CustomsCase customsCase);
    Task SaveChangesAsync();
}
