using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request);
        Task<PaymentDTO> GetPaymentAsync(int paymentId);
        Task<PaymentResponse> UpdatePaymentStatus(UpdatePaymentStatusDTO request);
        Task<PaymentResponse> CompleteCODPaymentAsync(CODPaymentUpdateDTO codPaymentUpdateDTO);
    }
}
