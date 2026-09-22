using System;
using System.Threading.Tasks;
using Declaration.Domain.Entities;
using Declaration.Application.Interfaces;
using Declaration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Declaration.Infrastructure.Repositories;

public class CustomsDeclarationRepository : ICustomsDeclarationRepository
{
    private readonly DeclarationDbContext _context;

    public CustomsDeclarationRepository(DeclarationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomsDeclaration> GetByIdAsync(Guid id)
    {
        return await _context.CustomsDeclarations.FindAsync(id);
    }

    public async Task<CustomsDeclaration> GetByIdAndTenantAsync(Guid id, Guid tenantId)
    {
        return await _context.CustomsDeclarations
            .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);
    }

    public async Task AddAsync(CustomsDeclaration declaration)
    {
        await _context.CustomsDeclarations.AddAsync(declaration);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CustomsDeclaration declaration)
    {
        _context.CustomsDeclarations.Update(declaration);
        await _context.SaveChangesAsync();
    }
}
