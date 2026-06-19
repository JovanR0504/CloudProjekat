namespace SmartMetering.Infrastructure.Settings;

public class StripeSettings
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
}
