using System.Linq.Expressions;

namespace MyWebApi.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate);
    }
}
