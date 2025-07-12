using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Providers.IProviders;
using MyWebApi.Repository;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using Stripe;
using Stripe.Checkout;
using PaymentMethod = MyWebApi.Enums.PaymentMethod;

namespace MyWebApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        public PaymentService(IUnitOfWork unitOfWork, IPaymentRepository repository, IEmailService emailService, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _paymentRepo = repository;
            _emailService = emailService;
            _db = db;

        }

        public async Task<PaymentResponse> CompleteCODPaymentAsync(CODPaymentUpdateDTO codPaymentUpdateDTO)
        {
            using (var transaction =  _db.Database.BeginTransaction())
            {
                try
                {
                    var payment = await _paymentRepo.GetPaymentByOrderIdAsync(codPaymentUpdateDTO.orderId);
                    if (payment == null) 
                    {
                        return new PaymentResponse
                        {
                            isSuccess = false,
                            Message = "Không tìm thấy payment"
                        };
                    }
                    if (payment.Order == null)
                    {
                        return new PaymentResponse
                        {
                            isSuccess = false,
                            Message = "Không tìm thấy đơn hàng thanh toán với phương thức này"
                        };
                    }
                    if (payment.Order.Status != OrderStatus.Shipping)
                    {
                        return new PaymentResponse
                        {
                            isSuccess = false,
                            Message = $"Order cannot be marked as Delivered from {payment.Order.Status} State"
                        };
                    }
                    if (payment.Method != PaymentMethod.COD)
                    {
                        return new PaymentResponse
                        {
                            isSuccess = false,
                            Message = "Phương thức không phải tiền mặt"
                        };
                    }
                    payment.Status = PaymentStatus.Paid;
                    payment.Order.Status = OrderStatus.Delivered;
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new PaymentResponse
                    {
                        isSuccess = true,
                        Message = "Đơn hàng đã được giao và thanh toán bằng tiền mặt"
                    };

                }
                catch (Exception ex)
                {
                    LogException.LogExceptions(ex);
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<PaymentDTO> GetPaymentAsync(int paymentId)
        {
            try
            {

                var payment = await _paymentRepo.GetPaymentByIdAsync(paymentId);
                if (payment == null)
                {
                    throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
                }
                var paymentDTO = new PaymentDTO(payment.Id,
                    payment.OrderId,
                    payment.Method,
                    payment.TransactionId,
                    payment.Amount,
                    payment.Status);
                return paymentDTO;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving the payment.", ex);
            }
        }


        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequestDTO request)
        {
            return new PaymentResponse
            {

            };
            
        }

        public async Task<PaymentResponse> UpdatePaymentStatus(UpdatePaymentStatusDTO request)
        {
            try
            {


                var payment = await _paymentRepo.GetPaymentByOrderIdAsync(request.paymentId);
                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Payment not found",
                    };
                }
                if (payment.Status == PaymentStatus.Paid && payment.Method != PaymentMethod.COD)
                {
                    payment.TransactionId = request.TransactionId;
                    payment.Order.Status = OrderStatus.Processing;
                }
                await _unitOfWork.SaveAsync();
                if (payment.Order.Status == OrderStatus.Processing)
                {
                    await _emailService.SendEmailAsync(payment.CustomerEmail, "Thông báo", "Đơn hàng đã được giao");
                }
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Cập nhật trạng thái thanh toán thành công"
                };
            }
            catch (Exception ex) {
                LogException.LogExceptions(ex);
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Lỗi khi thanh toán"
                };
             }
            
        }  

        





        //private async Task<Response> ProcessStripePaymentAsync(int amount, int userId)
        //{
        //    try
        //    {
        //        var options = new SessionCreateOptions
        //        {
        //            //SuccessUrl = "https://yourdomain.com/success",
        //            LineItems = new List<SessionLineItemOptions>(),
        //            Mode = "payment",
        //        };
        //        var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
        //        var cartDTO = new CartDTO(cart.Id,
        //            cart.CartItems.Select(ci => new CartItemDTO(
        //                ci.Id,
        //                ci.ProductId,
        //                ci.CartId,
        //                ci.Price,
        //                ci.Quantity
        //                )).ToList(),
        //            cart.TotalPrice,
        //            DateTime.UtcNow );

        //        foreach (var items in cartDTO.CartItemsDTO)
        //        {
        //            var sessionLineItem = new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = items.productId.ToString(), // Assuming productId is a string representation of the product name
        //                    },
        //                    UnitAmount = items.Price * 100, // Convert to cents
        //                },
        //                Quantity = items.Quantity,
        //            }; 
        //            options.LineItems.Add(sessionLineItem);
        //        }  
        //        var service = new SessionService();
        //        Session session = await service.CreateAsync(options);
        //        var order = _orderService.GetOrderByIdAsync(cart.UserId);
        //        await _orderService.UpdateStripePaymentIdAsync(order.Id, session.Id, session.PaymentIntentId);
        //        await _unitOfWork.SaveAsync();

        //        return new Response(true, "Thanh toán thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException.LogExceptions(ex);
        //        return new Response(false, "Thanh toán thất bại");
        //    }
        //}
    }
}
