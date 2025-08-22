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
using Stripe.Climate;
using PaymentMethod = MyWebApi.Enums.PaymentMethod;

namespace MyWebApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        private readonly IStripeProvider _stripeProvider;

        public PaymentService(IUnitOfWork unitOfWork, IPaymentRepository repository, IEmailService emailService, ApplicationDbContext db, IStripeProvider stripeProvider)
        {
            _unitOfWork = unitOfWork;
            _paymentRepo = repository;
            _emailService = emailService;
            _db = db;
            _stripeProvider = stripeProvider;
        }
        public async Task<PaymentResponse> ProcessPaymentCallbackAsync(int paymentId, PaymentCallBack callbackRequest)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var payment = await _db.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.Id == paymentId);

                if (payment == null)
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Không tìm thấy thông tin thanh toán",
                        status = PaymentStatus.Failed,
                        Data = new { }
                    };
                }

                // Xử lý callback theo gateway
                var result = await ProcessCallbackByGateway(payment);

                if (result.isSuccess)
                {
                    // Cập nhật payment
                    payment.Status = PaymentStatus.Paid;
                    payment.TransactionId = callbackRequest.TransactionId;

                    //await _unitOfWork.Orders.UpdateAsync(payment.Order);
                    // Cập nhật order status
                    await UpdateOrderPaymentStatus(payment.OrderId, PaymentStatus.Paid);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return new PaymentResponse
                    {
                        isSuccess = true,
                        Message = "Thanh toán thành công",
                        TransactionId = payment.TransactionId ?? string.Empty,
                        status = PaymentStatus.Paid,
                        Data = new
                        {
                            PaymentId = payment.Id,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount
                        }
                    };
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return result;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
               
                LogException.LogExceptions(ex);

                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Lỗi khi xử lý callback thanh toán",
                    status = PaymentStatus.Failed,
                    Data = new { }
                };
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
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = await _db.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == request.orderId);
                if (order == null)
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Không tìm thấy đơn hàng"
                    };
                }
                if (order.PaymentStatus == PaymentStatus.Paid)
                {
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Đơn hàng đã được thanh toán"
                    };
                }
                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount, 
                    Currency = "VND",
                    Status = PaymentStatus.Pending,
                    Method = request.paymentMethod,
                    TransactionId = string.Empty,
                    CreatedAt = DateTime.Now,
                    CustomerEmail = request.customerEmail,
                };
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveAsync();
                //xử lý thanh toán theo phương thức Stripe, COD, ... -> return PaymentResponse
                var result = await ProcessPaymentByMethod(payment, request);
                if (result.isSuccess)
                {
                    payment.Status = result.status;
                    payment.TransactionId = result.TransactionId;

                    // Nếu là COD thì cập nhật trạng thái ngay
                    if (request.paymentMethod == PaymentMethod.COD)
                    {
                        payment.Order.Status = OrderStatus.Confirmed;
                        payment.Status = PaymentStatus.Unpaid;
                    }
                    await _unitOfWork.Payments.UpdatePaymentAsync(payment);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return new PaymentResponse
                    {
                        isSuccess = true,
                        Message = result.Message,
                        TransactionId = result.TransactionId,
                        PaymentUrl = result.PaymentUrl,
                        status = result.status,
                        Data = new
                        {
                            PaymentId = payment.Id,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount,
                            Currency = payment.Currency,
                            CreatedAt = payment.CreatedAt,
                            CustomerEmail = payment.CustomerEmail
                        }
                    };
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;

                    await _unitOfWork.SaveAsync();
                    await transaction.RollbackAsync();

                    return result;
                }
            }
            catch (Exception e) {
                await transaction.RollbackAsync();
                LogException.LogExceptions(e);
                return new PaymentResponse
                {
                    isSuccess = false,
                    Message = "Lỗi khi tạo thanh toán",
                    status = PaymentStatus.Failed,
                    Data = new { }
                };


            } 
            
        }

        //xử lý thanh toán theo phương thức
        private async Task<PaymentResponse> ProcessPaymentByMethod(Payment payment, PaymentRequestDTO request)
        {
            switch (payment.Method)
            {
                case PaymentMethod.COD:
                    return new PaymentResponse
                    {
                        isSuccess = true,
                        Message = "Đặt hàng thành công - Thanh toán khi nhận hàng",
                        TransactionId = $"COD_{payment.Id}_{DateTime.UtcNow.Ticks}",
                        status = PaymentStatus.Pending,
                        PaymentUrl = string.Empty,
                        Data = new { PaymentMethod = "COD" }
                    };

                case PaymentMethod.Stripe:
                    return await _stripeProvider.CreatePaymentIntentAsync(payment, request.ReturnUrl, request.CancelUrl);

                default:
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Phương thức thanh toán không được hỗ trợ",
                        status = PaymentStatus.Failed,
                        Data = new { }
                    };
            }
        }

        public async Task<PaymentResponse> UpdatePaymentStatus(UpdatePaymentStatusDTO request)
        {
            try
            {


                var payment = await _paymentRepo.GetPaymentByIdAsync(request.paymentId);
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
               
                if (payment.Order.Status == OrderStatus.Processing)
                {
                    payment.Order.Status = OrderStatus.Delivered;
                    await _emailService.SendEmailAsync(payment.CustomerEmail, "Thông báo", "Đơn hàng đã được giao");
                }
                await _unitOfWork.Payments.UpdatePaymentAsync(payment);
                await _unitOfWork.SaveAsync();
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

        #region PRIVATE
        //xử lý callback từ các gateway thanh toán
        private async Task<PaymentResponse> ProcessCallbackByGateway(Payment payment) //, PaymentCallBack callbackRequest)
        {
            switch (payment.Method)
            {
                case PaymentMethod.Stripe:
                    return await _stripeProvider.HandleCallbackAsync(payment);

                case PaymentMethod.COD:
                    // COD payment
                    return new PaymentResponse
                    {
                        isSuccess = true,
                        Message = "Đơn hàng sẽ được thanh toán khi nhận hàng",
                        status = PaymentStatus.Unpaid,
                        Data = new { }
                    };
                //case PaymentMethod.VNPay:
                //case PaymentMethod.MoMo:
                default:
                    return new PaymentResponse
                    {
                        isSuccess = false,
                        Message = "Phương thức thanh toán không được hỗ trợ",
                        status = PaymentStatus.Failed,
                        Data = new { }
                    };
            }
        }
        private async Task UpdateOrderPaymentStatus(int orderId, PaymentStatus paymentStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus;

                // Cập nhật order status tương ứng
                if (paymentStatus == PaymentStatus.Paid)
                {
                    order.Status = OrderStatus.Confirmed;
                }

                order.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync();

                
                
            }
        }
        #endregion







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

