namespace SmartMetering.Application.Interfaces;

public interface IEmailService
{
    Task SendActivationEmailAsync(string toEmail, string firstName, string activationToken);
    Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetToken);
    Task SendInvoiceEmailAsync(string toEmail, string firstName, string invoicePeriod, decimal totalAmount);
    Task SendLimitExceededEmailAsync(string toEmail, string firstName, decimal currentUsage, decimal threshold, string unit);
}
