using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;

namespace Identity.Infrastructure.Repositories;

public class IdentityRepository : IIdentityRepository
{
    private readonly IdentityDbContext _context;

    public IdentityRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string email, Guid tenantId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
    }

    public async Task<User> GetUserByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
