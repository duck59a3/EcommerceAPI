using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;

namespace MyWebApi.Services
{
    public class ProductService : GenericService<Product>, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _repository;
        public ProductService(IUnitOfWork unitOfWork, IProductRepository repository) : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var existingProduct = await _repository.GetByIdAsync(entity.Id);
                if (existingProduct == null)
                {
                    return new Response(false, $"Không tìm thấy sản phẩm {entity.Name} để cập nhật");
                }
                //_repository.Entry(existingProduct).State = EntityState.Detached; // Detach the existing entity
                existingProduct.Name = entity.Name;
                existingProduct.Description = entity.Description;
                existingProduct.Price = entity.Price;
                existingProduct.Quantity = entity.Quantity;
                existingProduct.Size = entity.Size;
                existingProduct.Color = entity.Color;
                existingProduct.Material = entity.Material;
                existingProduct.CategoryId = entity.CategoryId;
                existingProduct.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(existingProduct);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Cập nhật thất bại");
            }
        }
    }   

}
