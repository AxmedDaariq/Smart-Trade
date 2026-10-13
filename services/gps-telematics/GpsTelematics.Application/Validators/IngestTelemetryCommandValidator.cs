using FluentValidation;
using GpsTelematics.Application.Commands;

namespace GpsTelematics.Application.Validators;

public class IngestTelemetryCommandValidator : AbstractValidator<IngestTelemetryCommand>
{
    public IngestTelemetryCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.DeviceId).NotEmpty();

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90.0, 90.0)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180.0, 180.0)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.Speed).GreaterThanOrEqualTo(0).WithMessage("Speed cannot be negative.");
        RuleFor(x => x.RecordedAt).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("RecordedAt cannot be in the future.");
    }
}
