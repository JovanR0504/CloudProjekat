namespace SmartMetering.Contracts.Admin;

public record AdminInvoiceSummaryResponse(
    int TotalInvoices,
    int PaidCount,
    int UnpaidCount,
    decimal TotalBilled,
    decimal TotalCollected);
