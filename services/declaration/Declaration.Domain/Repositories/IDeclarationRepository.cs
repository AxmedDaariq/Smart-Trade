using System;
using System.Threading.Tasks;
using Declaration.Domain.Entities;

namespace Declaration.Domain.Repositories;

public interface IDeclarationRepository
{
    Task<CustomsDeclaration> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(CustomsDeclaration declaration);
    Task UpdateAsync(CustomsDeclaration declaration);
    Task SaveChangesAsync();
}
