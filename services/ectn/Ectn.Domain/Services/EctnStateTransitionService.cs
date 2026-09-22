using System;
using System.Threading.Tasks;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;

namespace Ectn.Domain.Services;

public interface IEctnStateTransitionService
{
    Task ValidateTransitionAsync(Domain.Entities.Ectn ectn, EctnStatus targetStatus, Guid actorId);
}

public class EctnStateTransitionService : IEctnStateTransitionService
{
    public Task ValidateTransitionAsync(Domain.Entities.Ectn ectn, EctnStatus targetStatus, Guid actorId)
    {
        // Enforce strict Domain Invariants (Rule 83)
        switch (targetStatus)
        {
            case EctnStatus.Submitted:
                if (ectn.Status != EctnStatus.Draft)
                    throw new InvalidOperationException("ECTN must be in Draft state to be submitted.");
                if (ectn.Parties.Count == 0)
                    throw new InvalidOperationException("ECTN must have at least one party defined before submission.");
                break;
                
            case EctnStatus.Approved:
                if (ectn.Status != EctnStatus.UnderReview && ectn.Status != EctnStatus.Submitted)
                    throw new InvalidOperationException("ECTN must be Submitted or Under Review to be approved.");
                break;
                
            case EctnStatus.Issued:
                if (ectn.Status != EctnStatus.Approved)
                    throw new InvalidOperationException("ECTN must be Approved before it can be Issued.");
                if (string.IsNullOrWhiteSpace(ectn.EctnNumber))
                    throw new InvalidOperationException("ECTN must have an official number assigned before issuance.");
                break;

            case EctnStatus.Rejected:
                if (ectn.Status == EctnStatus.Issued || ectn.Status == EctnStatus.Expired || ectn.Status == EctnStatus.Cancelled)
                    throw new InvalidOperationException("Cannot reject an ECTN that is already Issued, Cancelled, or Expired.");
                break;
        }

        return Task.CompletedTask;
    }
}
