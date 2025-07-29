using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Providers.IProviders;
using MyWebApi.Responses;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;

namespace MyWebApi.Providers
{
    public class StripeProvider : IStripeProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StripeProvider(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaymentResponse> CreatePaymentIntentAsync(Payment payment, string? returnUrl, string? cancelUrl)
        {
            try
            {
                var domain = "https://localhost:7186/";
                var options = new SessionCreateOptions {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)payment.Amount,
                                Currency = payment.Currency.ToLower(),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    
                                    Name = $"Order #{payment.OrderId}",
                                    Description = $"Payment for order {payment.OrderId}"
                                
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    
                    SuccessUrl = returnUrl ?? domain + $"payment/success?payment_id={payment.Id}",
                    CancelUrl = cancelUrl ?? domain + $"payment/cancel?payment_id={payment.Id}",
                    ClientReferenceId = payment.Id.ToString(),
                    Metadata = new Dictionary<string, string>
                    {
                        ["payment_id"] = payment.Id.ToString(),
                        ["order_id"] = payment.OrderId.ToString(),
                        ["customer_email"] = payment.CustomerEmail
                    }
                    


                };
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return new PaymentResponse
                {
                    isSuccess = true,
                    Message = "Stripe payment session created successfully",
                    TransactionId = session.Id,
                    PaymentUrl = session.Url,
                    status = PaymentStatus.Pending,
                    Data = new
                    {
                        SessionId = session.Id,
                        ExpiresAt = session.ExpiresAt
                    }
                };
            }
            catch (StripeException ex) {
                LogException.LogExceptions(ex);
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = $"Stripe error: {ex.Message}",
                    status = PaymentStatus.Failed,
                    Data = new { StripeError = ex.StripeError }
                };
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Error creating payment session",
                    status = PaymentStatus.Failed,
                    Data = new { }
                };
            }
        }

        public async Task<PaymentResponse> HandleCallbackAsync(Payment payment, string webhookPayload)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null)
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Invalid request context",
                        status = PaymentStatus.Failed,
                        Data = new { }
                    };
                }
                var stripeSignature = request.Headers["Stripe-Signature"].FirstOrDefault();
                if (string.IsNullOrEmpty(stripeSignature))
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Không có Stripe signature",
                        status = PaymentStatus.Failed,
                        Data = new { }
                    };
                }
                var stripeEvent = EventUtility.ConstructEvent(
                    webhookPayload,
                    stripeSignature,
                    _configuration["Stripe:WebhookSecret"]
                );
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed": 
                        var session = stripeEvent.Data.Object as Session;

                        if (session?.ClientReferenceId == payment.Id.ToString())
                        {
                            return new PaymentResponse
                            {
                                isSuccess = true,
                                Message = "Payment completed successfully",
                                TransactionId = session.Id,
                                status = PaymentStatus.Paid,
                                Data = new
                                {
                                    StripeSessionId = session.Id,
                                    AmountTotal = session.AmountTotal,
                                    PaymentStatus = session.PaymentStatus
                                }
                            };
                        }
                        break;

                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                        if (paymentIntent?.Metadata.ContainsKey("payment_id") == true &&
                            paymentIntent.Metadata["payment_id"] == payment.Id.ToString())
                        {
                            return new PaymentResponse
                            {
                                isSuccess = true,
                                Message = "Payment intent succeeded",
                                TransactionId = paymentIntent.Id,
                                status = PaymentStatus.Paid,
                                Data = new
                                {
                                    PaymentIntentId = paymentIntent.Id,
                                    Amount = paymentIntent.Amount
                                }
                            };
                        }
                        break;

                    case "payment_intent.payment_failed":
                        var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                        if (failedPaymentIntent?.Metadata.ContainsKey("payment_id") == true &&
                            failedPaymentIntent.Metadata["payment_id"] == payment.Id.ToString())
                        {
                            return new PaymentResponse
                            {
                                isSuccess = false,
                                Message = "Payment failed",
                                TransactionId = failedPaymentIntent.Id,
                                status = PaymentStatus.Failed,
                                Data = new
                                {
                                    PaymentIntentId = failedPaymentIntent.Id,
                                    LastPaymentError = failedPaymentIntent.LastPaymentError?.Message
                                }
                            };
                        }
                        break;

                    default:
                        
                        break;
                }
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Invalid webhook event",
                    status = PaymentStatus.Failed,
                    Data = new { }
                };
            }
            catch (StripeException ex)
            {
               LogException.LogExceptions(ex);

                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = $"Stripe webhook error: {ex.Message}",
                    status = PaymentStatus.Failed,
                    Data = new { StripeError = ex.StripeError }
                };
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Error processing webhook",
                    status = PaymentStatus.Failed,
                    Data = new { }
                };
            }
        }
    }
}
