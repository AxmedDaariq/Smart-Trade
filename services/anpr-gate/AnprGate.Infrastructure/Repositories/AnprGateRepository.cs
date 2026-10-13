using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AnprGate.Domain.Entities;
using AnprGate.Domain.Repositories;
using AnprGate.Infrastructure.Persistence;

namespace AnprGate.Infrastructure.Repositories;

public class AnprGateRepository : IAnprGateRepository
{
    private readonly AnprDbContext _context;

    public AnprGateRepository(AnprDbContext context)
    {
        _context = context;
    }

    public async Task<RecognitionEvent> GetRecognitionByIdAsync(Guid id)
    {
        return await _context.RecognitionEvents.FindAsync(id);
    }

    public async Task AddRecognitionAsync(RecognitionEvent recognition)
    {
        await _context.RecognitionEvents.AddAsync(recognition);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
