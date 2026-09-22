using FluentValidation;

namespace Ectn.Application.Commands;

public class CreateEctnCommandValidator : AbstractValidator<CreateEctnCommand>
{
    public CreateEctnCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty().WithMessage("TenantId is required.");
        RuleFor(x => x.ApplicationReference).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
