using MyWebApi.Responses;
using System.Linq.Expressions;
namespace MyWebApi.Services.IService
{
    public interface IGenericService<T> where T : class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);


    }
}
