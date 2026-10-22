using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Declaration.Domain.Entities;
using Declaration.Domain.Repositories;
using Declaration.Infrastructure.Persistence;

namespace Declaration.Infrastructure.Repositories;

public class DeclarationRepository : IDeclarationRepository
{
    private readonly DeclarationDbContext _context;

    public DeclarationRepository(DeclarationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomsDeclaration> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.CustomsDeclarations.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);
    }

    public async Task AddAsync(CustomsDeclaration declaration)
    {
        await _context.CustomsDeclarations.AddAsync(declaration);
    }

    public async Task UpdateAsync(CustomsDeclaration declaration)
    {
        _context.CustomsDeclarations.Update(declaration);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
