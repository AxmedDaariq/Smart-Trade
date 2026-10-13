using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Customs.Domain.Entities;
using Customs.Domain.Repositories;
using Customs.Infrastructure.Persistence;

namespace Customs.Infrastructure.Repositories;

public class CustomsRepository : ICustomsRepository
{
    private readonly CustomsDbContext _context;

    public CustomsRepository(CustomsDbContext context)
    {
        _context = context;
    }

    public async Task<CustomsCase> GetCaseByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.CustomsCases.FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
    }

    public async Task AddAsync(CustomsCase customsCase)
    {
        await _context.CustomsCases.AddAsync(customsCase);
    }

    public async Task UpdateAsync(CustomsCase customsCase)
    {
        _context.CustomsCases.Update(customsCase);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
