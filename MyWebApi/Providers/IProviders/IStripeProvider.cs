using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Responses;
using Stripe;

namespace MyWebApi.Providers.IProviders
{
    public interface IStripeProvider
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request, Payment payment);
        Task<PaymentResponse> VerifyPaymentAsync(string transactionId);
    }
}
