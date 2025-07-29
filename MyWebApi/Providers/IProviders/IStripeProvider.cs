using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Responses;
using Stripe;

namespace MyWebApi.Providers.IProviders
{
    public interface IStripeProvider
    {
        Task<PaymentResponse> CreatePaymentIntentAsync(Payment payment, string? returnUrl, string? cancelUrl);
        Task<PaymentResponse> HandleCallbackAsync(Payment payment, string webhookPayload);
    }
}
