using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartMetering.Application.Interfaces;
using SmartMetering.Infrastructure.Settings;

namespace SmartMetering.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SendGridSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendActivationEmailAsync(string toEmail, string firstName, string activationToken)
    {
        var activationUrl = $"{_settings.FrontendBaseUrl}/activate?token={activationToken}";
        var subject = "Activate Your SmartMetering Account";
        var body = $"Hello {firstName},<br><br>" +
                   $"Your account has been created. Please activate it by clicking the link below:<br>" +
                   $"<a href=\"{activationUrl}\">Activate Account</a><br><br>" +
                   $"This link expires in 24 hours.";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetToken)
    {
        var resetUrl = $"{_settings.FrontendBaseUrl}/reset-password?token={resetToken}";
        var subject = "Reset Your Password";
        var body = $"Hello {firstName},<br><br>" +
                   $"You requested a password reset. Click the link below:<br>" +
                   $"<a href=\"{resetUrl}\">Reset Password</a><br><br>" +
                   $"This link expires in 1 hour. If you didn't request this, ignore this email.";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendInvoiceEmailAsync(string toEmail, string firstName, string invoicePeriod, decimal totalAmount)
    {
        var subject = $"Your Invoice for {invoicePeriod}";
        var body = $"Hello {firstName},<br><br>" +
                   $"Your invoice for <strong>{invoicePeriod}</strong> has been generated.<br>" +
                   $"Total amount: <strong>{totalAmount:N2} RSD</strong><br><br>" +
                   $"You can view and pay your invoice from your dashboard.";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendLimitExceededEmailAsync(string toEmail, string firstName, decimal currentUsage, decimal threshold, string unit)
    {
        var subject = "Consumption Limit Exceeded";
        var body = $"Hello {firstName},<br><br>" +
                   $"Your current consumption of <strong>{currentUsage:N2} {unit}</strong> has exceeded " +
                   $"your set limit of <strong>{threshold:N2} {unit}</strong>.<br><br>" +
                   $"Please review your usage from the dashboard.";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) || _settings.ApiKey == "your-sendgrid-api-key")
        {
            _logger.LogWarning(
                "[DEV EMAIL] To: {To}, Subject: {Subject}, Body: {Body}",
                toEmail, subject, htmlContent);
            return;
        }

        var client = new SendGridClient(_settings.ApiKey);
        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email to {Email}. Status: {Status}", toEmail, response.StatusCode);
        }
    }
}
