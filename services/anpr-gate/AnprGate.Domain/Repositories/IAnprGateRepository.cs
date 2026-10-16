using System;
using System.Threading.Tasks;
using AnprGate.Domain.Entities;

namespace AnprGate.Domain.Repositories;

public interface IAnprGateRepository
{
    Task<AnprRecognitionEvent?> GetRecognitionByIdAsync(Guid id);
    Task AddRecognitionAsync(AnprRecognitionEvent recognition);
    Task SaveChangesAsync();
}
