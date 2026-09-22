using System.Threading.Tasks;
using System;

namespace Declaration.Application.Interfaces;

public interface ICustomsServiceClient
{
    Task SubmitForAssessmentAsync(Guid declarationId, Guid tenantId);
}
