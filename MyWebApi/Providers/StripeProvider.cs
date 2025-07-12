using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Providers.IProviders;
using MyWebApi.Responses;
using Stripe;

namespace MyWebApi.Providers
{
    public class StripeProvider : IStripeProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;

        public StripeProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request, Payment payment)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.amount * 100), // Convert to cents
                    Metadata = new Dictionary<string, string>
                    {
                        {"order_id", request.orderId.ToString()},
                        {"payment_id", payment.Id.ToString()}
                    }
                };

                if (!string.IsNullOrEmpty(request.customerEmail))
                {
                    options.ReceiptEmail = request.customerEmail;
                }

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return new PaymentResponse
                {
                    isSuccess = true,
                    Message = "Stripe payment intent created successfully",
                    TransactionId = paymentIntent.Id,
                    status = PaymentStatus.Pending,
                    Data = new
                    {
                        client_secret = paymentIntent.ClientSecret,
                        payment_intent_id = paymentIntent.Id
                    }
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = $"Stripe payment failed: {ex.Message}",
                    status = PaymentStatus.Failed
                };
            }
        }

        public async Task<PaymentResponse> VerifyPaymentAsync(string transactionId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(transactionId);

                var status = paymentIntent.Status switch
                {
                    "succeeded" => PaymentStatus.Paid,
                    "processing" => PaymentStatus.Pending,
                    "requires_payment_method" => PaymentStatus.Failed,
                    "canceled" => PaymentStatus.Failed,
                    _ => PaymentStatus.Pending
                };

                return new PaymentResponse
                {
                    isSuccess = status == PaymentStatus.Paid,
                    Message = $"Payment status: {paymentIntent.Status}",
                    TransactionId = transactionId,
                    status = status
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = $"Stripe verification failed: {ex.Message}",
                    status = PaymentStatus.Failed
                };
            }
        }
    }
}
