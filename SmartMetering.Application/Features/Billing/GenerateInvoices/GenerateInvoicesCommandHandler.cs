using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Billing;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Billing.GenerateInvoices;

public class GenerateInvoicesCommandHandler
    : IRequestHandler<GenerateInvoicesCommand, Result<GenerateInvoicesResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ITelemetryStore _telemetryStore;
    private readonly IStorageService _storageService;
    private readonly IEmailService _emailService;
    private readonly ILogger<GenerateInvoicesCommandHandler> _logger;

    public GenerateInvoicesCommandHandler(
        IAppDbContext context,
        ITelemetryStore telemetryStore,
        IStorageService storageService,
        IEmailService emailService,
        ILogger<GenerateInvoicesCommandHandler> logger)
    {
        _context = context;
        _telemetryStore = telemetryStore;
        _storageService = storageService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<GenerateInvoicesResponse>> Handle(
        GenerateInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        // Get the active tariff model for the billing period
        var billingDate = new DateOnly(request.Year, request.Month, 1);

        var tariff = await _context.TariffModels
            .Where(t => t.IsActive)
            .Where(t => t.EffectiveFrom <= billingDate)
            .Where(t => t.EffectiveTo == null || t.EffectiveTo >= billingDate)
            .OrderByDescending(t => t.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (tariff is null)
        {
            return Result<GenerateInvoicesResponse>.Failure(
                Error.NotFound("Billing.NoActiveTariff",
                    $"No active tariff model found for {request.Month:D2}/{request.Year}."));
        }

        // Get all meters that are not Unregistered (i.e., have been paired at some point)
        var meters = await _context.Meters
            .Where(m => m.Status != MeterStatus.Unregistered)
            .ToListAsync(cancellationToken);

        // Get existing invoices for this period to detect duplicates
        var existingInvoices = await _context.Invoices
            .Where(i => i.Month == request.Month && i.Year == request.Year)
            .Select(i => i.MeterId)
            .ToListAsync(cancellationToken);

        var existingMeterIds = new HashSet<Guid>(existingInvoices);

        int generated = 0;
        int skipped = 0;
        int failed = 0;

        foreach (var meter in meters)
        {
            try
            {
                // Skip if invoice already exists for this meter/month/year
                if (existingMeterIds.Contains(meter.Id))
                {
                    skipped++;
                    continue;
                }

                // Get the property to find the user
                var property = await _context.Properties
                    .FirstOrDefaultAsync(p => p.Id == meter.PropertyId, cancellationToken);

                if (property is null)
                {
                    _logger.LogWarning(
                        "Meter {MeterId} has no associated property. Skipping invoice generation.",
                        meter.Id);
                    skipped++;
                    continue;
                }

                // Get monthly VT and NT consumption
                var (highTariffKwh, lowTariffKwh) = await _telemetryStore
                    .GetMonthlyConsumptionAsync(meter.Id, request.Month, request.Year);

                var totalKwh = highTariffKwh + lowTariffKwh;

                // Calculate billing
                var billing = BillingCalculator.Calculate(
                    highTariffKwh,
                    lowTariffKwh,
                    tariff,
                    meter.ApprovedPowerKw);

                // Create invoice via domain factory
                var invoiceResult = Invoice.Create(
                    property.UserId,
                    property.Id,
                    meter.Id,
                    request.Month,
                    request.Year,
                    highTariffKwh,
                    lowTariffKwh,
                    totalKwh,
                    billing.GreenZoneKwh,
                    billing.BlueZoneKwh,
                    billing.RedZoneKwh,
                    billing.EnergyCharge,
                    billing.PowerCharge,
                    billing.SupplierCharge,
                    billing.TotalAmount);

                if (invoiceResult.IsFailure)
                {
                    _logger.LogWarning(
                        "Failed to create invoice for meter {MeterId}: {Error}",
                        meter.Id, invoiceResult.Error!.Message);
                    failed++;
                    continue;
                }

                var invoice = invoiceResult.Value;

                // Generate invoice document and upload to blob storage
                var documentContent = GenerateInvoiceDocument(invoice, meter, property);
                var blobPath = $"{property.UserId}/{request.Year}/{request.Month:D2}/{invoice.Id}.txt";

                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(documentContent));
                await _storageService.UploadBlobAsync("invoices", blobPath, stream, "text/plain");

                invoice.SetDocumentPath(blobPath);

                _context.Invoices.Add(invoice);

                // Send email notification to consumer
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == property.UserId, cancellationToken);

                if (user is not null)
                {
                    var invoicePeriod = $"{request.Month:D2}/{request.Year}";
                    await _emailService.SendInvoiceEmailAsync(
                        user.Email,
                        user.FirstName,
                        invoicePeriod,
                        billing.TotalAmount);
                }

                generated++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating invoice for meter {MeterId} for period {Month}/{Year}.",
                    meter.Id, request.Month, request.Year);
                failed++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var message = $"Invoice generation completed: {generated} generated, {skipped} skipped, {failed} failed.";
        _logger.LogInformation(message);

        return Result<GenerateInvoicesResponse>.Success(
            new GenerateInvoicesResponse(generated, skipped, failed, message));
    }

    private static string GenerateInvoiceDocument(Invoice invoice, Meter meter, Property property)
    {
        var sb = new StringBuilder();
        sb.AppendLine("===========================================");
        sb.AppendLine("         SMART METERING INVOICE");
        sb.AppendLine("===========================================");
        sb.AppendLine();
        sb.AppendLine($"Invoice ID:     {invoice.Id}");
        sb.AppendLine($"Consumer ID:    {invoice.UserId}");
        sb.AppendLine($"Property ID:    {invoice.PropertyId}");
        sb.AppendLine($"Property:       {property.Name}, {property.Address}, {property.City}");
        sb.AppendLine($"Meter ID:       {invoice.MeterId}");
        sb.AppendLine($"Meter SN:       {meter.SerialNumber}");
        sb.AppendLine();
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine("  BILLING PERIOD");
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine($"Month/Year:     {invoice.Month:D2}/{invoice.Year}");
        sb.AppendLine();
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine("  CONSUMPTION");
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine($"VT (High Tariff):  {invoice.HighTariffKwh:F2} kWh");
        sb.AppendLine($"NT (Low Tariff):   {invoice.LowTariffKwh:F2} kWh");
        sb.AppendLine($"Total:             {invoice.TotalKwh:F2} kWh");
        sb.AppendLine();
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine("  ZONE BREAKDOWN");
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine($"Green Zone:        {invoice.GreenZoneKwh:F2} kWh");
        sb.AppendLine($"Blue Zone:         {invoice.BlueZoneKwh:F2} kWh");
        sb.AppendLine($"Red Zone:          {invoice.RedZoneKwh:F2} kWh");
        sb.AppendLine();
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine("  CHARGES");
        sb.AppendLine("-------------------------------------------");
        sb.AppendLine($"Energy Charge:     {invoice.EnergyCharge:F2} RSD");
        sb.AppendLine($"Power Charge:      {invoice.PowerCharge:F2} RSD");
        sb.AppendLine($"Supplier Charge:   {invoice.SupplierCharge:F2} RSD");
        sb.AppendLine();
        sb.AppendLine("===========================================");
        sb.AppendLine($"  TOTAL AMOUNT:    {invoice.TotalAmount:F2} RSD");
        sb.AppendLine("===========================================");
        sb.AppendLine();
        sb.AppendLine($"Status:            {invoice.Status}");
        sb.AppendLine($"Generated:         {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

        return sb.ToString();
    }
}
