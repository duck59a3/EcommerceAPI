using MyWebApi.DTOs;
using MyWebApi.Helpers.QueryParameters;
using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IProductService : IGenericService<Product>
    {
       Task<Response> UpdateAsync(Product entity);
       Task<PagedResponse<Product>> GetProductsAsync(ProductQuery query);
       Task<Response> UploadProductImage(int productId,IFormFile file);
       Task<Response> UploadMultipleProductImages(int productId,IFormFileCollection files);
       Task<Response> DeleteProductImage(string publicId);
       Task<string> GetOptimizedImageUrl(string publicId, int width = 0, int height = 0);
        Task<Response> UploadProductVideo(int productId,IFormFile file);


    }
}
