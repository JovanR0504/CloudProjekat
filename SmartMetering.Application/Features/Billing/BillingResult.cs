namespace SmartMetering.Application.Features.Billing;

public record BillingResult(
    decimal GreenZoneKwh,
    decimal BlueZoneKwh,
    decimal RedZoneKwh,
    decimal EnergyCharge,
    decimal PowerCharge,
    decimal SupplierCharge,
    decimal TotalAmount);
