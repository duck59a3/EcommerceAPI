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
        [HttpPost("process")]
        public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] PaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request == null)
            {
                return BadRequest("Invalid payment request.");
            }
            var response = await _paymentService.ProcessPaymentAsync(request);
            return Ok(response);
        }
        //[HttpGet("verify/{transactionId}")]
        //public async Task<ActionResult<PaymentResponse>> VerifyPayment(string transactionId, [FromQuery] PaymentMethod method)
        //{
        //    var response = await _paymentService.VerifyPaymentAsync(transactionId, method);
        //    return Ok(response);
        //}
        [HttpPost("call-back")]
        public async Task<ActionResult<PaymentResponse>> PaymentCallback([FromQuery] int paymentId, [FromBody] PaymentCallBack req)
        {
            if (paymentId <= 0 || req == null)
                return BadRequest("Thiếu thông tin callback");

            var result = await _paymentService.ProcessPaymentCallbackAsync(paymentId, req);

            if (!result.isSuccess)
                return BadRequest(result);

            return Ok(result);
        }



    }
}


