namespace SmartMetering.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(
        Guid invoiceId,
        decimal amount,
        string description,
        string successUrl,
        string cancelUrl);
}
