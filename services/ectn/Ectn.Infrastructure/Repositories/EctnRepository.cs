using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EctnEntity = Ectn.Domain.Entities.Ectn;
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

    public async Task<EctnEntity?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Ectns
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
    }

    public async Task AddAsync(EctnEntity ectn)
    {
        await _context.Ectns.AddAsync(ectn);
    }

    public async Task UpdateAsync(EctnEntity ectn)
    {
        _context.Ectns.Update(ectn);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
