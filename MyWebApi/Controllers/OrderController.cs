using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Message = "Không tìm thấy đơn hàng" });
            }
            return Ok(order);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummaryDTO>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound(new { Message = "Không tìm thấy đơn hàng cho người dùng này" });
            }
            return Ok(orders);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderSummaryDTO>>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                return NotFound(new { Message = "Không tìm thấy đơn hàng cho người dùng này" });
            }
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrderFromCart(CreateOrderDTO createOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Hoan thanh Authorize => thay thế userId bằng userId từ token
            var response = await _orderService.CreateOrderFromCartAsync(createOrderDTO.userId, createOrderDTO.paymentMethod, createOrderDTO.notes);
            return Ok(response);
        }
    }
}
