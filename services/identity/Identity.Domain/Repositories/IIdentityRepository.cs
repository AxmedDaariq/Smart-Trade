using System;
using System.Threading.Tasks;
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IIdentityRepository
{
    Task<User> GetUserByEmailAsync(string email, Guid tenantId);
    Task<User> GetUserByIdAsync(Guid id, Guid tenantId);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task SaveChangesAsync();
}
