namespace SmartMetering.Infrastructure.Settings;

public class SendGridSettings
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; } = null!;
    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = null!;
    public string FrontendBaseUrl { get; set; } = null!;
}
