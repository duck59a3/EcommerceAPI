using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Conversions;
using MyWebApi.DTOs.Requests;
using MyWebApi.Enums;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Services
{
    public class OrderService : GenericService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public OrderService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ApplicationDbContext context, IEmailService emailService) : base(unitOfWork, orderRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _emailService = emailService;
        }
        public async Task<Response> CreateOrderAsync(CreateOrderDTO createOrderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync() ;
            try
            {
                int totalAmount = 0;
                var orderItems = new List<OrderDetail>();
                foreach (var item in createOrderDto.items)
                {
                    var product = await _context.Products.FindAsync(item.productId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product with ID {item.productId} not found.");
                    }
                    if (product.Quantity < item.count)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");
                    }
                    totalAmount += product.Price * item.count;
                    orderItems.Add(new OrderDetail
                    {
                        ProductId = item.productId,
                        ProductName = item.productName,
                        Count = item.count,
                        Price = product.Price
                    });
                    product.Quantity -= item.count; // Giảm số lượng sản phẩm trong kho
                }
                int ShippingCost = CalculateShippingCost(totalAmount);
                int grandTotal = totalAmount + ShippingCost;
                var order = new Order
                {
                    UserId = createOrderDto.userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    ShippingCost = ShippingCost,
                    GrandTotal = grandTotal,
                    PaymentMethod = createOrderDto.paymentMethod,
                    PaymentStatus = PaymentStatus.Pending,
                    Notes = createOrderDto.notes,
                    FullName = createOrderDto.name,
                    Phone = createOrderDto.phoneNumber,
                    Address = createOrderDto.address,
                    City = createOrderDto.city,
                    Items = orderItems
                };
                await _orderRepository.AddAsync(order);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                return new Response(true, "Tạo đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogException.LogExceptions(ex);
                return new Response(false, $"Có lõi khi tạo đơn hàng {ex.Message}.");
            }
        }

        public async Task<Response> CreateOrderFromCartAsync(int userId, string paymentMethod, string notes = null!)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.CartItems.Any())
                {
                    return new Response(false, "Giỏ hàng trống hoặc không tồn tại.");
                }
                int totalAmount = 0;
                var orderItems = new List<OrderDetail>();
                var orderCreateDto = new CreateOrderDTO(userId,
                    cart.AppUser.Name,
                    cart.AppUser.PhoneNumber!,
                    cart.AppUser.Address!,
                    cart.AppUser.City!,
                    notes,
                    paymentMethod,
                    cart.CartItems.Select(ci => new CreateOrderDetailDTO(ci.ProductId,ci.ProductName, ci.Quantity)).ToList()

                    );

                foreach (var item in orderCreateDto.items)
                {
                    var product = await _context.Products.FindAsync(item.productId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product with ID {item.productId} not found.");
                    }
                    if (product.Quantity < item.count)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");
                    }
                    totalAmount += product.Price * item.count;
                    orderItems.Add(new OrderDetail
                    {
                        ProductId = item.productId,
                        ProductName = item.productName,
                        Count = item.count,
                        Price = product.Price
                    });
                    product.Quantity -= item.count; // Giảm số lượng sản phẩm trong kho
                }
                int ShippingCost = CalculateShippingCost(totalAmount);
                int grandTotal = totalAmount + ShippingCost;
                var order = new Order
                {
                    UserId = orderCreateDto.userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Confirmed,
                    TotalAmount = totalAmount,
                    ShippingCost = ShippingCost,
                    GrandTotal = grandTotal,
                    PaymentMethod = orderCreateDto.paymentMethod,
                    PaymentStatus = PaymentStatus.Pending,
                    Notes = orderCreateDto.notes,
                    FullName = orderCreateDto.name,
                    Phone = orderCreateDto.phoneNumber,
                    Address = orderCreateDto.address,
                    City = orderCreateDto.city,
                    Items = orderItems
                };
                await _orderRepository.AddAsync(order);
                await _unitOfWork.Carts.RemoveAsync(cart); 
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                await _emailService.SendEmailAsync("pokiwarduc@gmail.com", "Thông báo", "Đơn hàng được đặt thành công");
                return new Response(true, "Tạo đơn hàng từ giỏ hàng thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogException.LogExceptions(ex);
                //return new Response(false, $"Có lỗi khi tạo đơn hàng từ giỏ hàng: {ex.Message}.");
                return new Response(false, $"Có lỗi khi tạo đơn hàng từ giỏ hàng: {(ex.InnerException?.Message ?? ex.Message)}.");
            }
        
        }

        public Task<Response> DeleteOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }

                var (orderDto,_) = OrderConversion.FromEntity(order, null!);
                return orderDto;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving the order.", ex);
            } 
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                if (orders == null)
                {
                    throw new KeyNotFoundException("No orders found.");
                }
                var (_, orderDtos) = OrderConversion.FromEntity(null, orders);
                return orderDtos;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving orders.", ex);
            }
        }

        public Task<List<OrderDTO>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                if (orders == null || !orders.Any())
                {
                    throw new KeyNotFoundException($"No orders found for user with ID {userId}.");
                }

                var orderSummaries = orders.Select(o => new OrderSummaryDTO(o.Id,o.OrderDate,o.Status,o.GrandTotal));
                
                return orderSummaries;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving orders by user ID.", ex);
            }
        }
        public async Task<Response> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO updateStatusDto)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }
                order.Status = updateStatusDto.orderStatus;
                order.PaymentStatus = updateStatusDto.paymentStatus;
                order.UpdatedAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(updateStatusDto.notes))
                {
                    order.Notes = updateStatusDto.notes;
                }
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Cập nhật trạng thái đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Lỗi cập nhật trạng thái đơn hàng.");

            }

        }

        public async Task<Response> UpdateStripePaymentIdAsync(int Id, string sessionId, string paymentIntentId)
        {
            var order = await _orderRepository.GetByIdAsync(Id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                order.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                order.paymentIntentId = paymentIntentId;
            }
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            return new Response(true, "Cập nhật thông tin thanh toán Stripe thành công.");
        }

        private int CalculateShippingCost(int totalAmount)
        {
            if (totalAmount > 500000) return 0;
            if (totalAmount > 200000) return 20000;
            return 30000;

        }
    }
}
