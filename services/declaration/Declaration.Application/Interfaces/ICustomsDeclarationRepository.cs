using System;
using System.Threading.Tasks;
using Declaration.Domain.Entities;

namespace Declaration.Application.Interfaces;

public interface ICustomsDeclarationRepository
{
    Task<CustomsDeclaration> GetByIdAsync(Guid id);
    Task<CustomsDeclaration> GetByIdAndTenantAsync(Guid id, Guid tenantId);
    Task AddAsync(CustomsDeclaration declaration);
    Task UpdateAsync(CustomsDeclaration declaration);
}
