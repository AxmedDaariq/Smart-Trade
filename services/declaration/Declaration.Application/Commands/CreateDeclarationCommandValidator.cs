using FluentValidation;

namespace Declaration.Application.Commands;

public class CreateDeclarationCommandValidator : AbstractValidator<CreateDeclarationCommand>
{
    public CreateDeclarationCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId is required.");
        RuleFor(x => x.ApplicationReference).NotEmpty().WithMessage("ApplicationReference is required.");
        RuleFor(x => x.ProcedureCode).NotEmpty().WithMessage("ProcedureCode is required.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreatedBy is required.");
        RuleFor(x => x.DeclarationType).IsInEnum().WithMessage("Invalid DeclarationType.");
    }
}
