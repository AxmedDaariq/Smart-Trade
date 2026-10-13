using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ectn.Domain.Entities;
using Ectn.Domain.Repositories;
using Ectn.Infrastructure.Persistence;

namespace Ectn.Infrastructure.Repositories;

public class EctnRepository : IEctnRepository
{
    private readonly EctnDbContext _context;

    public EctnRepository(EctnDbContext context)
    {
        _context = context;
    }

    public async Task<Ectn> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Ectns
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
    }

    public async Task AddAsync(Ectn ectn)
    {
        await _context.Ectns.AddAsync(ectn);
    }

    public async Task UpdateAsync(Ectn ectn)
    {
        _context.Ectns.Update(ectn);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
