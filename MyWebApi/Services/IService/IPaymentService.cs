using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request); //create

        Task<PaymentResponse> ProcessPaymentCallbackAsync(int paymentId, PaymentCallBack callbackRequest);


        Task<PaymentDTO> GetPaymentAsync(int paymentId);
        Task<PaymentResponse> UpdatePaymentStatus(UpdatePaymentStatusDTO request);
    }
}
