using FluentValidation;

namespace SmartMetering.Application.Features.Meters.CreateMeter;

public class CreateMeterCommandValidator : AbstractValidator<CreateMeterCommand>
{
    public CreateMeterCommandValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("Property ID is required.");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Label is required.")
            .MaximumLength(150).WithMessage("Label must not exceed 150 characters.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.");

        RuleFor(x => x.ConnectionType)
            .NotEmpty().WithMessage("Connection type is required.");
    }
}
