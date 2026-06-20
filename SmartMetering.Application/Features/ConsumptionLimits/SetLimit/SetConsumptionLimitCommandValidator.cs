using FluentValidation;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.ConsumptionLimits.SetLimit;

public class SetConsumptionLimitCommandValidator : AbstractValidator<SetConsumptionLimitCommand>
{
    public SetConsumptionLimitCommandValidator()
    {
        RuleFor(x => x.Threshold)
            .GreaterThan(0).WithMessage("Threshold must be greater than zero.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .Must(u => Enum.TryParse<LimitUnit>(u, ignoreCase: true, out _))
            .WithMessage("Unit must be 'Kwh' or 'Rsd'.");
    }
}
