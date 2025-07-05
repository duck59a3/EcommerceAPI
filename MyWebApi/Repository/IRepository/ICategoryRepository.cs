using MyWebApi.Models;

namespace MyWebApi.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task UpdateAsync(Category category);
    }
}
