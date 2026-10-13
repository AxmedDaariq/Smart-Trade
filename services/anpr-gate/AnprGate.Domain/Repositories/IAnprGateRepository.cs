using System;
using System.Threading.Tasks;
using AnprGate.Domain.Entities;

namespace AnprGate.Domain.Repositories;

public interface IAnprGateRepository
{
    Task<RecognitionEvent> GetRecognitionByIdAsync(Guid id);
    Task AddRecognitionAsync(RecognitionEvent recognition);
    Task SaveChangesAsync();
}
