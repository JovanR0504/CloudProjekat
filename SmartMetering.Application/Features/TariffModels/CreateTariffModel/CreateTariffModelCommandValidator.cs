using FluentValidation;

namespace SmartMetering.Application.Features.TariffModels.CreateTariffModel;

public class CreateTariffModelCommandValidator : AbstractValidator<CreateTariffModelCommand>
{
    public CreateTariffModelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tariff model name is required.")
            .MaximumLength(200).WithMessage("Tariff model name must not exceed 200 characters.");

        RuleFor(x => x.GreenHighTariffPrice)
            .GreaterThan(0).WithMessage("Green high tariff price must be greater than zero.");

        RuleFor(x => x.GreenLowTariffPrice)
            .GreaterThan(0).WithMessage("Green low tariff price must be greater than zero.");

        RuleFor(x => x.BlueHighTariffPrice)
            .GreaterThan(0).WithMessage("Blue high tariff price must be greater than zero.");

        RuleFor(x => x.BlueLowTariffPrice)
            .GreaterThan(0).WithMessage("Blue low tariff price must be greater than zero.");

        RuleFor(x => x.RedHighTariffPrice)
            .GreaterThan(0).WithMessage("Red high tariff price must be greater than zero.");

        RuleFor(x => x.RedLowTariffPrice)
            .GreaterThan(0).WithMessage("Red low tariff price must be greater than zero.");

        RuleFor(x => x.GreenZoneUpperLimit)
            .GreaterThan(0).WithMessage("Green zone upper limit must be greater than zero.");

        RuleFor(x => x.BlueZoneUpperLimit)
            .GreaterThan(0).WithMessage("Blue zone upper limit must be greater than zero.")
            .Must((cmd, blueLimit) => blueLimit > cmd.GreenZoneUpperLimit)
            .WithMessage("Blue zone upper limit must be greater than green zone upper limit.");

        RuleFor(x => x.PowerBillingRate)
            .GreaterThan(0).WithMessage("Power billing rate must be greater than zero.");

        RuleFor(x => x.SupplierFee)
            .GreaterThan(0).WithMessage("Supplier fee must be greater than zero.");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty().WithMessage("Effective from date is required.");
    }
}
