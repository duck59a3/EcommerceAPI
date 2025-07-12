
using MyWebApi.DTOs.Requests;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Providers.IProviders
{
    public interface ICODPaymentProvider
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request, Payment payment);
    }
}
