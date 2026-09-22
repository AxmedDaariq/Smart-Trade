using System;
using System.Threading.Tasks;

namespace Declaration.Application.Interfaces;

public interface IEctnServiceClient
{
    Task<bool> ValidateEctnAsync(string ectnReference, Guid tenantId);
}
