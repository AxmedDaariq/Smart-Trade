using System;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;

namespace Declaration.Domain.Policies;

public interface IDeclarationStateTransitionPolicy
{
    bool CanTransition(CustomsDeclaration declaration, DeclarationStatus targetStatus);
    void EnforceTransitionRules(CustomsDeclaration declaration, DeclarationStatus targetStatus);
}
