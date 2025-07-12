using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        //[HttpPost("process")]
        //public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] PaymentRequestDTO request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (request == null)
        //    {
        //        return BadRequest("Invalid payment request.");
        //    }
        //    var response = await _paymentService.ProcessPaymentAsync(request);
        //    return Ok(response);
        //}
        //[HttpGet("verify/{transactionId}")]
        //public async Task<ActionResult<PaymentResponse>> VerifyPayment(string transactionId, [FromQuery] PaymentMethod method)
        //{
        //    var response = await _paymentService.VerifyPaymentAsync(transactionId, method);
        //    return Ok(response);
        //}
        //[HttpGet("{transactionId}")]
        //public async Task<ActionResult<Payment>> GetPayment(string transactionId)
        //{
        //    var payment = await _paymentService.GetPaymentAsync(transactionId);
        //    if (payment == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(payment);
        //}
        //[HttpPost("stripe/webhook")]
        //public async Task<IActionResult> StripeWebhook()
        //{

        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        //    // Process Stripe webhook
        //    var stripeEvent = Stripe.EventUtility.ConstructEvent(json,
        //        Request.Headers["Stripe-Signature"],
        //        "whsec_kj6r6QpjYBajPecZuVn8l9GrCDFByUMz");

        //    if (stripeEvent.Type == "payment_intent.succeeded")
        //    {
        //        var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
        //        await _paymentService.VerifyPaymentAsync(paymentIntent.Id, PaymentMethod.Stripe);
        //    }

        //    return Ok();
        //}

    }
}


