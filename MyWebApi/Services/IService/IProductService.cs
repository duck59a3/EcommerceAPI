using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface IProductService : IGenericService<Product>
    {
       Task<Response> UpdateAsync(Product entity);
    }
}
