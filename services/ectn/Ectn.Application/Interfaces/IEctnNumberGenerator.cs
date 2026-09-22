using System;
using System.Threading.Tasks;

namespace Ectn.Application.Interfaces;

public interface IEctnNumberGenerator
{
    Task<string> GenerateOfficialNumberAsync(Guid tenantId, string portOfLoading, DateTime issueDate);
}
