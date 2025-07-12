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
       Task<Response> UploadProductImage(int productId, List<IFormFile> files);
    }
}
