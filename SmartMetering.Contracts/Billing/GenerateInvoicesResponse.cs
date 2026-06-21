namespace SmartMetering.Contracts.Billing;

public record GenerateInvoicesResponse(
    int GeneratedCount,
    int SkippedCount,
    int FailedCount,
    string Message);
