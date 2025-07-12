using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Providers.IProviders;
using MyWebApi.Responses;

namespace MyWebApi.Providers
{
    public class CODPaymentProvider : ICODPaymentProvider
    {
        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request, Payment payment)
        {
            return await Task.FromResult(new PaymentResponse
            {
                isSuccess = true,
                Message = "Cash payment processed successfully",
                TransactionId = payment.Id.ToString(),
                status = PaymentStatus.Paid,
            });
        }
    }
}
