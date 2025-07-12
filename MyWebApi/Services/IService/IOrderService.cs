using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IOrderService : IGenericService<Order>
    {
        Task<Response> CreateOrderAsync(CreateOrderDTO createOrderDto);
        Task<Response> CreateOrderFromCartAsync(int userId, string paymentMethod, string notes);
        Task<OrderDTO> GetOrderByIdAsync(int orderId);

        Task<Response> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO updateStatusDto);
        Task<Response> UpdateStripePaymentIdAsync(int Id, string sessionId, string paymentIntentId);
        Task<List<OrderDTO>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<Response> DeleteOrderAsync(int orderId);
        Task<IEnumerable<OrderDTO>> GetOrdersAsync();
        Task<IEnumerable<OrderSummaryDTO>> GetOrdersByUserIdAsync(int userId);
    }
}
