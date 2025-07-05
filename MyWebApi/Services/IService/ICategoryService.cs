using MyWebApi.Models;
using MyWebApi.Responses;

namespace MyWebApi.Services.IService
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<Response> UpdateAsync(Category category);
    }
}
