using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class Invoice : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid MeterId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public decimal HighTariffKwh { get; private set; }
    public decimal LowTariffKwh { get; private set; }
    public decimal TotalKwh { get; private set; }
    public decimal GreenZoneKwh { get; private set; }
    public decimal BlueZoneKwh { get; private set; }
    public decimal RedZoneKwh { get; private set; }
    public decimal EnergyCharge { get; private set; }
    public decimal PowerCharge { get; private set; }
    public decimal SupplierCharge { get; private set; }
    public decimal TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public string? DocumentPath { get; private set; }
    public string? StripeSessionId { get; private set; }
    public string? StripePaymentId { get; private set; }

    private Invoice() { }

    public static Result<Invoice> Create(
        Guid userId,
        Guid propertyId,
        Guid meterId,
        int month,
        int year,
        decimal highTariffKwh,
        decimal lowTariffKwh,
        decimal totalKwh,
        decimal greenZoneKwh,
        decimal blueZoneKwh,
        decimal redZoneKwh,
        decimal energyCharge,
        decimal powerCharge,
        decimal supplierCharge,
        decimal totalAmount)
    {
        if (userId == Guid.Empty)
            return Result<Invoice>.Failure(Error.Validation("Invoice.EmptyUserId", "User ID is required."));

        if (month < 1 || month > 12)
            return Result<Invoice>.Failure(Error.Validation("Invoice.InvalidMonth", "Month must be between 1 and 12."));

        if (year < 2020)
            return Result<Invoice>.Failure(Error.Validation("Invoice.InvalidYear", "Year must be 2020 or later."));

        var invoice = new Invoice
        {
            UserId = userId,
            PropertyId = propertyId,
            MeterId = meterId,
            Month = month,
            Year = year,
            HighTariffKwh = highTariffKwh,
            LowTariffKwh = lowTariffKwh,
            TotalKwh = totalKwh,
            GreenZoneKwh = greenZoneKwh,
            BlueZoneKwh = blueZoneKwh,
            RedZoneKwh = redZoneKwh,
            EnergyCharge = energyCharge,
            PowerCharge = powerCharge,
            SupplierCharge = supplierCharge,
            TotalAmount = totalAmount,
            Status = InvoiceStatus.Unpaid
        };

        return Result<Invoice>.Success(invoice);
    }

    public void MarkPaid(string stripePaymentId)
    {
        Status = InvoiceStatus.Paid;
        PaidAtUtc = DateTime.UtcNow;
        StripePaymentId = stripePaymentId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetDocumentPath(string path)
    {
        DocumentPath = path;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetStripeSessionId(string sessionId)
    {
        StripeSessionId = sessionId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
