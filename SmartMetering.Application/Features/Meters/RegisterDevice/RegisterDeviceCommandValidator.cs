using FluentValidation;

namespace SmartMetering.Application.Features.Meters.RegisterDevice;

public class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("Device ID is required.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.");
    }
}
