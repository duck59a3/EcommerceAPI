using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Requests;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Services
{
    public class CartService : GenericService<Cart>, ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly ICartRepository _repository;
        private readonly IProductRepository _productRepository;
        public CartService(IUnitOfWork unitOfWork, ICartRepository repository, IProductRepository productRepository, ApplicationDbContext db) : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _productRepository = productRepository;
            _db = db;
        }

        public async Task<Response> AddToCartAsync(int userId, AddToCartDTO request)
        {
            try
            {
                var cart = await _repository.GetCartByUserIdAsync(userId);
                var product = await _productRepository.GetByIdAsync(request.productId);
                if (product == null)
                {
                    return new Response(false, "Sản phẩm không tồn tại");
                }
                if (product.Quantity < request.Quantity)
                {
                    return new Response(false, "Số lượng sản phẩm không đủ trong kho");
                }
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _repository.AddAsync(cart);
                    await _unitOfWork.SaveAsync();
                }
                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += request.Quantity;
                    existingItem.Price = existingItem.Product.Price * existingItem.Quantity;
                }
                else
                {

                    var cartItem = new CartItem
                    {
                        ProductId = request.productId,
                        CartId = cart.Id,
                        Quantity = request.Quantity,
                        Price = product.Price * request.Quantity
                    };
                    cart.CartItems.Add(cartItem);
                }
                await _unitOfWork.SaveAsync();
                return new Response(true, "Thêm sản phẩm vào giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Lỗi khi thêm sản phẩm vào giỏ hàng");
            }
        }

        public async Task<decimal> CalculateTotalPriceAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return 0; // Return 0 if the cart or cart items are null/empty
            }

            var totalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity); // Access CartItems after ensuring cart is retrieved
            return totalPrice;

        }

        public Task<Response> ClearCartAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDTO> GetCartByUserIdAsync(int userId)
        {
            try
            {
                var cart = await _repository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _repository.AddAsync(cart);
                    await _unitOfWork.SaveAsync();
                }

                var cartDTO = new CartDTO(cart.Id,
                    cart.CartItems.Select(i => new CartItemDTO(i.Id, i.ProductId, i.CartId, i.Price, i.Quantity)).ToList(),
                    cart.TotalPrice,
                    DateTime.UtcNow


                );
                return cartDTO;

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Lỗi khi lấy giỏ hàng");
            }
        }

        public async Task<Response> RemoveFromCartAsync(int userId, int cartItemId)
        {
            try
            {
                var cart = await _repository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return new Response(false, "Giỏ hàng không tồn tại");
                }
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                {
                    return new Response(false, "Mặt hàng trong giỏ hàng không tồn tại");
                }
                _db.CartItems.Remove(cartItem);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Mặt hàng đã được xóa khỏi giỏ hàng");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Lỗi khi xóa mặt hàng khỏi giỏ hàng");
            }
        }

        public async Task<Response> UpdateCartAsync(int userId, int cartItemId, UpdateCartDTO updateCartDTO)
        {
            try
            {
                var cart = await _repository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    throw new InvalidOperationException("Giỏ hàng không tồn tại");
                }
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                {
                    throw new InvalidOperationException("Mặt hàng trong giỏ hàng không tồn tại");
                }
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product.Quantity < updateCartDTO.Quantity)
                {
                    throw new InvalidOperationException("Số lượng không đủ");
                }
                cartItem.Quantity = updateCartDTO.Quantity;
                _db.CartItems.Update(cartItem);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Cập nhật giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Lỗi khi cập nhật giỏ hàng: {ex.Message}");

            }
        }
        
    }
}
