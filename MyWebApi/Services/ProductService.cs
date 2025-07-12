using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Conversions;
using MyWebApi.Helpers.QueryParameters;
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
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductService(IUnitOfWork unitOfWork, IProductRepository repository, ApplicationDbContext db, IWebHostEnvironment env) : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _db = db;
            _env = env;
        }

        public async Task<Response> UploadProductImage(int productId, List<IFormFile> files)
        {
            try
            {
                string wwwrootPath = _env.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"Images\products\product-" + productId;
                        string finalPath = Path.Combine(wwwrootPath, productPath);
                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        var image = new ProductImage
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productId,
                        };
                        await _unitOfWork.ProductImages.AddAsync(image);
                        //var existproduct = await _repository.GetByIdAsync(product.Id);
                        //existproduct

                    }
                    await _unitOfWork.SaveAsync();
                    return new Response(true, "Thêm hình ảnh sản phẩm thành công");
                }
                return new Response(false, "Không có file nào được tải lên");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Lỗi khi up hình ảnh {ex.Message}");
            }
        }
        

        public async Task<PagedResponse<Product>> GetProductsAsync(ProductQuery query)
        {
            //Filtering
            var products = _db.Products.AsQueryable();
            if (query.CategoryId.HasValue)
                products = products.Where(p => p.CategoryId == query.CategoryId);

            if (query.MinPrice.HasValue)
                products = products.Where(p => p.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= query.MaxPrice.Value);

            if (!string.IsNullOrEmpty(query.Search))
                products = products.Where(p => p.Name.Contains(query.Search));
            //Sorting
            products = query.SortBy?.ToLower() switch
            {
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "name" => products.OrderBy(p => p.Name),
                "name_desc" => products.OrderByDescending(p => p.Name),
                _ => products.OrderBy(p => p.Id)
            };

            //  Pagination
            var total = await products.CountAsync();
            var items = await products
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResponse<Product>
            (
                items,
                query.PageNumber,
                query.PageSize,
                total

            );
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
